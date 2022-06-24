using Dirt.ProcGen;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static Dirt.ProcGen.GenerativeGraph;

namespace ProcGenEditor
{
    public class ProcGenGraphView : GraphView
    {
        public RuntimeGraph GraphInstance { get; set; }
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
                }
            }

            if ( changes.elementsToRemove != null )
            {
                for(int i = 0; i < changes.elementsToRemove.Count; ++i)
                {
                    var elem = changes.elementsToRemove[i];
                    if ( elem is ProcGenGraphNodeView nodeView)
                    {
                        ArrayUtility.Remove(ref GraphInstance.Nodes, nodeView.Node);
                    }
                    if ( elem is Edge edge )
                    {
                        ProcGenGraphNodeView inputNode = (ProcGenGraphNodeView) edge.input.node;
                        inputNode.TryGetPortData(edge.input, out int edgeSlot, out _);
                        inputNode.Node.Inputs[edgeSlot].Connect(null, 0);
                    }
                }
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