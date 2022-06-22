using Newtonsoft.Json;
using ProcGen.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Dirt.ProcGen
{
    [CreateAssetMenu(fileName = "gen_graph.asset", menuName ="Dirt/Generative Graph")]
    public class GenerativeGraph : ScriptableObject
    {
        //public BaseNode[] Nodes = new BaseNode[0];
        public string SerializedGraph = "{}";
        public string[] NodeTypes = new string[0];
        public NodeConnection[] Connections = new NodeConnection[0];

        public void SerializeGraph(RuntimeGraph graph, JsonSerializerSettings settings)
        {
            List<NodeConnection> connections = new List<NodeConnection>();

            // nodes, edges, editor meta
            SerializedGraph = JsonConvert.SerializeObject(graph, settings);
            NodeTypes = new string[graph.Nodes.Length];
            for(int i = 0; i < graph.Nodes.Length; ++i)
            {
                BaseNode node = graph.Nodes[i];
                NodeTypes[i] = node.GetType().FullName;
                AddNodeConnections(graph.Nodes, node, connections);
            }

            Connections = connections.ToArray();
        }

        private static void AddNodeConnections(BaseNode[] nodeArray, BaseNode node, List<NodeConnection> connections)
        {
            int nodeIndex = System.Array.IndexOf(nodeArray, node);

            for(int i = 0; node.Inputs != null && i < node.Inputs.Length; ++i)
            {
                NodeConnector input = node.Inputs[i];
                if ( input.IsConnectorValid())
                {
                    int outputNode = System.Array.IndexOf(nodeArray, input.Source);
                    connections.Add(new NodeConnection(nodeIndex, i, outputNode, input.SourceOutputIndex));
                }
            }
        }

        public RuntimeGraph Deserialize(JsonSerializerSettings settings, BaseNodeConverter nodeConverter)
        {
            nodeConverter.SetTypeArray(NodeTypes);
            RuntimeGraph graph = JsonConvert.DeserializeObject<RuntimeGraph>(SerializedGraph, settings);
            graph.Initialize();
            for(int i = 0; i < Connections.Length; ++i)
            {
                NodeConnection connection = Connections[i];
                BaseNode outputNode = graph.Nodes[connection.OutputNode];
                BaseNode inputNode = graph.Nodes[connection.InputNode];
                inputNode.Inputs[connection.InputSlot].Connect(outputNode, connection.OutputSlot);
            }
            return graph;
        }


        [System.Serializable]
        public struct NodeConnection
        {
            public int InputNode, OutputNode;
            public int InputSlot, OutputSlot;

            public NodeConnection(int inputNode, int inputSlot, int outputNode, int outputSlot)
            {
                InputNode = inputNode;
                InputSlot = inputSlot;
                OutputNode = outputNode;
                OutputSlot = outputSlot;
            }
        }
    }
}
