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
        /// Invoked in editor, when a change to the graph has been made (can be used to regenerate scene after changes)
        /// </summary>
        public System.Action OnGraphUpdate;

        /// <summary>
        /// Call this to generate a usable runtime graph.
        /// </summary>
        public void GenerateRuntime()
        {
            Runtime = Graph.Deserialize(ProcGenSerialization.SerializationSettings, ProcGenSerialization.NodeConverter);
        }
    }
}