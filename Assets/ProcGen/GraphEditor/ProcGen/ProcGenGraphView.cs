using ProcGen;
using ProcGen.Connector;
using ProcGen.Model;
using ProcGenEditor.GraphElems;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProcGenEditor
{
    using NodeMetadata = ProcGen.Model.NodeMetadata;

    public class ProcGenGraphView : GraphView
    {
        public System.Action NotifyGraphUpdate;
        public System.Action<GraphElement> NotifyElementInspect;
        public GenerativeGraphInstance GraphInstance { get; set; }

        public ProcGenGraphNodeView CurrentMasterNode { get; set; }

        public ProcGenGraphView()
        {
            GridBackground gridBg = new GridBackground();
            Insert(0, gridBg);
            graphViewChanged += OnGraphViewChanged;
        }

        public void Unload()
        {
            nodes.ForEach(node =>
            {
                ((ProcGenGraphNodeView) node).Free();
            });
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> listPort = new List<Port>();
            ProcGenGraphNodeView startNodeView = (ProcGenGraphNodeView)startPort.node;
            bool isStrict = startNodeView.Attribute.StrictConnections;

            ports.ForEach(p =>
            {
                ProcGenGraphNodeView otherNodeView = (ProcGenGraphNodeView)p.node;

                if ( p.orientation == startPort.orientation 
                    && p.direction != startPort.direction
                    && ConnectorHelper.CanConvert((ConnectorType)startPort.userData, (ConnectorType)p.userData, isStrict || otherNodeView.Attribute.StrictConnections))
                {

                    listPort.Add(p);
                }
            });

            return listPort;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange changes)
        {
            bool refreshGraph = false;
            for (int i = 0; changes.edgesToCreate != null && i < changes.edgesToCreate.Count; ++i)
            {
                Edge edge = changes.edgesToCreate[i];
                ProcGenGraphNodeView sourceNodeView = (ProcGenGraphNodeView)edge.output.node;
                ProcGenGraphNodeView nodeView = (ProcGenGraphNodeView)edge.input.node;
                BaseNode sourceNode = sourceNodeView.Node;

                sourceNodeView.TryGetPortData(edge.output, out int sourceSlot, out _);
                nodeView.TryGetPortData(edge.input, out int slotIndex, out bool isOutput);

                if ( isOutput)
                {
                    Debug.LogError("Invalid connection");
                }
                else
                {
                    nodeView.Node.Inputs[slotIndex].Connect(sourceNode, sourceSlot);
                    edge.input.parent.Q<NodeInputView>()?.UpdateControlVisibility();
                    refreshGraph = true;
                }
            }

            if ( changes.elementsToRemove != null )
            {
                for(int i = 0; i < changes.elementsToRemove.Count; ++i)
                {
                    var elem = changes.elementsToRemove[i];
                    if ( elem is ProcGenGraphNodeView nodeView)
                    {
                        nodeView.Free();
                        int idx = nodeView.Node.NodeIndex;
                        ArrayUtility.Remove(ref GraphInstance.Runtime.Nodes, nodeView.Node);
                        // reupdate all node ids above
                        for (int j = idx; j < GraphInstance.Runtime.Nodes.Length; ++j)
                            GraphInstance.Runtime.Nodes[j].SetIndex(j);

                        refreshGraph = true;
                    }
                    if ( elem is Edge edge )
                    {
                        ProcGenGraphNodeView inputNode = (ProcGenGraphNodeView) edge.input.node;
                        if ( inputNode != null)
                        {
                            inputNode.TryGetPortData(edge.input, out int edgeSlot, out _);
                            inputNode.Node.Inputs[edgeSlot].Connect(null, 0);
                            edge.input.parent.Q<NodeInputView>()?.UpdateControlVisibility();
                        }
                        refreshGraph = true;
                    }
                }
            }

            if ( refreshGraph )
            {
                NotifyGraphUpdate?.Invoke();
            }
            return changes;
        }

        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            if (selectable is GraphElement graphElem)
                NotifyElementInspect?.Invoke(graphElem);
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if ( selection.Count > 0 && evt.target is ProcGenGraphNodeView )
            {
                evt.menu.AppendAction("Group Selection", (a) => 
                {
                    Group group = CreateGroup("New Group");
                    foreach(GraphElement elem in selection.OfType<ProcGenGraphNodeView>())
                    {
                        if ( elem is ProcGenGraphNodeView nodeView)
                        {
                            group.AddElement(nodeView);
                            nodeView.ParentGroup = group;
                        }
                    }
                });

                if ( selection.Count == 1 && selection[0] is ProcGenGraphNodeView nodeView)
                {
                    if ( nodeView.Node is IMasterNode masterNode)
                    {
                        evt.menu.AppendAction("Set Master", (a) =>
                        {
                            if (CurrentMasterNode != null)
                                CurrentMasterNode.RemoveFromClassList("masterNode");
                            nodeView.AddToClassList("masterNode");
                            GraphInstance.Runtime.SetMasterNode(nodeView.Node);
                            CurrentMasterNode = nodeView;
                        });
                    }
                }
            }
        }
        private void BuildGroupContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Delete Group", (a) => 
            {
                List<GraphElement> toRemove = new List<GraphElement>();
                HashSet<GraphElement> children = new HashSet<GraphElement>();
                foreach(var grp in selection.OfType<Group>())
                {
                    grp.CollectElements(children, (e) => true);
                    //grp.RemoveElements(children);  
                    grp.RemoveElements(children);
                    foreach(var child in children)
                    {
                        if (child is ProcGenGraphNodeView view)
                        {
                            view.ParentGroup = null;
                        }
                        AddElement(child);
                    }

                    toRemove.Add(grp);
                }
                for (int i = 0; i < toRemove.Count; ++i)
                    RemoveElement(toRemove[i]);
            });
        }

        public Group CreateGroup(string label)
        {
            Group group = new ProcGenGroup();
            group.title = label;
            group.AddManipulator(new ContextualMenuManipulator(BuildGroupContextualMenu));
            group.style.opacity = 1f;
            AddElement(group);
            return group;
        }


        public GraphMetadata SerializeGraphMetadata(RuntimeGraph graph)
        {
            var nodeViews = contentContainer.Query<ProcGenGraphNodeView>();
            var groups = contentContainer.Query<Group>();

            Dictionary<Group, int> groupIndices = new Dictionary<Group, int>();
            List<GroupMetadata> groupsMeta = new List<GroupMetadata>();
            NodeMetadata[] meta = new NodeMetadata[graph.Nodes.Length];

            groups.ForEach(group =>
            {
                int idx = groupsMeta.Count;
                groupIndices.Add(group, idx);
                groupsMeta.Add(new GroupMetadata()
                {
                    Title = group.title,
                    Color = ProcGenEditorHelper.BackgroundToColor(group)
                });
            });

            nodeViews.ForEach(n =>
            {
                int nodeIndex = System.Array.IndexOf(graph.Nodes, n.Node);
                int groupIdx;

                if ( n.ParentGroup == null || !groupIndices.TryGetValue(n.ParentGroup, out groupIdx))
                {
                    groupIdx = -1;
                }

               
                meta[nodeIndex] = new NodeMetadata()
                {
                    Position = n.GetPosition(),
                    GroupIndex = groupIdx
                };
            });


            return new GraphMetadata()
            {
                Nodes = meta,
                Groups = groupsMeta.ToArray()
            };
        }
    }
}