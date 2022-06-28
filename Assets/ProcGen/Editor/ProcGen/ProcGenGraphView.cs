using ProcGen;
using ProcGen.Connector;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProcGenEditor
{
    using NodeMetadata = GenerativeGraph.NodeMetadata;

    public class ProcGenGraphView : GraphView
    {
        public System.Action NotifyGraphUpdate;
        public GenerativeGraphInstance GraphInstance { get; set; }
        public ProcGenGraphView()
        {
            GridBackground gridBg = new GridBackground();
            Insert(0, gridBg);
            graphViewChanged += OnGraphViewChanged;
        }


        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> listPort = new List<Port>();

            ports.ForEach(p =>
            {
                if ( p.orientation == startPort.orientation 
                    && p.direction != startPort.direction
                    && ConnectorHelper.CanConvert((ConnectorType)startPort.userData, (ConnectorType)p.userData))
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
                        inputNode.TryGetPortData(edge.input, out int edgeSlot, out _);
                        inputNode.Node.Inputs[edgeSlot].Connect(null, 0);
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

        public NodeMetadata[] SerializeNodeMeta(RuntimeGraph graph)
        {
            var nodeViews = contentContainer.Query<ProcGenGraphNodeView>();
            NodeMetadata[] meta = new NodeMetadata[graph.Nodes.Length];

            nodeViews.ForEach(n =>
            {
                int nodeIndex = System.Array.IndexOf(graph.Nodes, n.Node);
                meta[nodeIndex] = new NodeMetadata()
                {
                    Position = n.GetPosition()
                };
            });
            return meta;
        }
    }
}