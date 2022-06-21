using Dirt.ProcGen;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ProcGenEditor
{
    public class ProcGenGraphView : GraphView
    {
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
                if ( p.orientation == startPort.orientation && p.direction != startPort.direction)
                {
                    listPort.Add(p);
                }
            });

            return listPort;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange changes)
        {
            for(int i = 0; changes.edgesToCreate != null && i < changes.edgesToCreate.Count; ++i)
            {
                Edge edge = changes.edgesToCreate[i];
                ProcGenGraphNodeView sourceNodeView = (ProcGenGraphNodeView)edge.output.node;
                ProcGenGraphNodeView nodeView = (ProcGenGraphNodeView)edge.input.node;
                BaseNode sourceNode = nodeView.Node;

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
            return changes;
        }
    }
}