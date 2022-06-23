using Newtonsoft.Json;
using ProcGen.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Dirt.ProcGen
{
    [CreateAssetMenu(fileName = "gen_graph.asset", menuName ="Dirt/Generative Graph")]
    public class GenerativeGraph : ScriptableObject
    {
        public string SerializedGraph = "{}";
        public string[] NodeTypes = new string[0];
        public NodeConnection[] Connections = new NodeConnection[0];
        public InputDefaultValue[] Values = new InputDefaultValue[0];
        public NodeMetadata[] Meta = new NodeMetadata[0];
        public void SerializeGraph(RuntimeGraph graph, JsonSerializerSettings settings)
        {
            List<NodeConnection> connections = new List<NodeConnection>();
            List<InputDefaultValue> values = new List<InputDefaultValue>();
            List<NodeMetadata> meta = new List<NodeMetadata>();

            // nodes, edges, editor meta
            SerializedGraph = JsonConvert.SerializeObject(graph, settings);
            NodeTypes = new string[graph.Nodes.Length];
            for(int i = 0; i < graph.Nodes.Length; ++i)
            {
                BaseNode node = graph.Nodes[i];
                NodeTypes[i] = node.GetType().FullName;
                AddNodeConnections(graph.Nodes, node, connections, values);

            }

            Connections = connections.ToArray();
            Values = values.ToArray();
        }


        private static void AddNodeConnections(BaseNode[] nodeArray, BaseNode node, List<NodeConnection> connections, List<InputDefaultValue> values)
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
                else
                {
                    // add default value?
                    values.Add(new InputDefaultValue(nodeIndex, i, input.Initial.InitialValueVector4));
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
                ref NodeConnection connection = ref Connections[i];
                BaseNode outputNode = graph.Nodes[connection.OutputNode];
                BaseNode inputNode = graph.Nodes[connection.InputNode];
                inputNode.Inputs[connection.InputSlot].Connect(outputNode, connection.OutputSlot);
            }
            for(int i = 0; i < Values.Length; ++i)
            {
                ref InputDefaultValue defValue = ref Values[i];
                BaseNode node = graph.Nodes[defValue.Node];
                node.Inputs[defValue.Slot].Initial.InitialValueVector4 = defValue.Value;

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

        [System.Serializable]
        public struct InputDefaultValue
        {
            public int Node;
            public int Slot;
            public Vector4 Value;

            public InputDefaultValue(int node, int slot, Vector4 value)
            {
                Node = node;
                Slot = slot;
                Value = value;
            }
        }

        [System.Serializable]
        public struct NodeMetadata
        {
            public Rect Position;
        }
    }
}
