using Newtonsoft.Json;
using ProcGen.Connector;
using ProcGen.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGen
{
    /// <summary>
    /// Generative Graph Unity Object. 
    /// This stores an editable procedural graph that can be then used at runtime for generating content.
    /// Content is not displayed unless the Debug option is ticked on in the inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "gen_graph.asset", menuName ="Dirt/Generative Graph")]
    public class GenerativeGraph : ScriptableObject
    {
        public string SerializedGraph = "{}";                   // Store polymorphic data (without actual types)
        public GraphReflection Reflection = GraphReflection.Empty; // store reflection data
        [SerializeField]  internal NodeConnection[] Connections = new NodeConnection[0]; // Store connection (out->in) between nodes
        [SerializeField]  internal InputDefaultValue[] Values = new InputDefaultValue[0]; // Store default value when an input has no output connected to it // TODO: Expose values in editor
        public NodeMetadata[] Meta = new NodeMetadata[0]; // Store meta data (editor position) for each nodes

        /// <summary>
        /// Call this to serialize changes made to the runtime graph and update the GenerativeGraph
        /// </summary>
        /// <param name="graph">runtime graph to serialize</param>
        /// <param name="settings">Newtonsoft settings to use for serialization</param>
        /// <seealso cref="ProcGenSerialization"/>
        public void SerializeGraph(RuntimeGraph graph, JsonSerializerSettings settings)
        {
            List<NodeConnection> connections = new List<NodeConnection>();
            List<InputDefaultValue> values = new List<InputDefaultValue>();
            List<NodeMetadata> meta = new List<NodeMetadata>();

            // nodes, edges, editor meta
            SerializedGraph = JsonConvert.SerializeObject(graph, settings);
            Reflection = GraphReflection.FromArray(graph.Nodes);
            for (int i = 0; i < graph.Nodes.Length; ++i)
            {
                BaseNode node = graph.Nodes[i];
                AddNodeConnections(graph.Nodes, node, connections, values);
            }

            Connections = connections.ToArray();
            Values = values.ToArray();
        }

        /// <summary>
        /// Call this if you want to deserialize with other settings
        /// </summary>
        /// <param name="settings">JsonSerializerSettings to use</param>
        /// <param name="nodeConverter">The type-to-object converted to use</param>
        /// <seealso cref="ProcGenSerialization"/>
        /// <returns>a runtime graph ready to evaluate</returns>
        public RuntimeGraph Deserialize(JsonSerializerSettings settings, BaseNodeConverter nodeConverter)
        {
            nodeConverter.SetGraphReflection(Reflection);
            RuntimeGraph graph = JsonConvert.DeserializeObject<RuntimeGraph>(SerializedGraph, settings);
            graph.Initialize();
            for (int i = 0; i < Connections.Length; ++i)
            {
                ref NodeConnection connection = ref Connections[i];
                BaseNode outputNode = graph.Nodes[connection.OutputNode];
                BaseNode inputNode = graph.Nodes[connection.InputNode];
                inputNode.Inputs[connection.InputSlot].Connect(outputNode, connection.OutputSlot);
            }
            for (int i = 0; i < Values.Length; ++i)
            {
                ref InputDefaultValue defValue = ref Values[i];
                BaseNode node = graph.Nodes[defValue.Node];
                node.Inputs[defValue.Slot].Initial.InitialValueVector4 = defValue.Value;

            }
            return graph;
        }

        private static void AddNodeConnections(BaseNode[] nodeArray, BaseNode node, List<NodeConnection> connections, List<InputDefaultValue> values)
        {
            int nodeIndex = System.Array.IndexOf(nodeArray, node);

            for(int i = 0; node.Inputs != null && i < node.Inputs.Length; ++i)
            {
                NodeInput input = node.Inputs[i];
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

        [System.Serializable]
        internal struct NodeConnection
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
        internal struct InputDefaultValue
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
