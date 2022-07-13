using ProcGen.Buffer;
using ProcGen.Connector;
using ProcGen.Serialization;

namespace ProcGen
{
    /// <summary>
    /// Graph and Runtime wrapper. Use this to quickly implements graphs.
    /// Objects can be inspected through the editor to attach and debug graphs.
    /// </summary>
    [System.Serializable]
    public class GenerativeGraphInstance
    {
        /// <summary>
        /// Generative Graph that will be read
        /// </summary>
        public GenerativeGraph Graph;
        /// <summary>
        /// Runtime graph is accessible after invoking GenerateRuntime()
        /// </summary>
        public RuntimeGraph Runtime { get; private set; }
        /// <summary>
        /// Raw buffer
        /// </summary>
        public GraphBuffer Buffer { get; private set; }

        private byte[] m_BufferBlock;

        /// <summary>
        /// Invoked in editor, when a change to the graph has been made (can be used to regenerate scene after changes)
        /// </summary>
        public System.Action OnGraphUpdate;

        /// <summary>
        /// Call this to generate a usable runtime graph.
        /// </summary>
        public void GenerateRuntime()
        {
            Runtime = Graph.Deserialize(ProcGenSerialization.SerializationSettings, ProcGenSerialization.NodeConverter);
            IMasterNode masterNode = Runtime.TargetNode != null ? Runtime.TargetNode as IMasterNode : null;
            if ( masterNode != null )
            {
                MasterNodeSettings settings = masterNode.GetSettings();
                if ( settings.HasBuffer )
                {
                    m_BufferBlock = new byte[GraphBuffer.ComputeBufferSize(1, settings.BufferLayout)];
                    Buffer = new GraphBuffer(m_BufferBlock, 0, settings.BufferLayout);
                    masterNode.SetBuffer(Buffer);
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Graph {Graph.name} has no master node");
            }
        }

        public void Clear()
        {
            Runtime = null;
            Buffer.FreePointer();
            m_BufferBlock = null;
        }
    }
}