using ProcGen.Serialization;

namespace ProcGen
{
    [System.Serializable]
    public class GenerativeGraphInstance
    {
        public GenerativeGraph Graph;
        public RuntimeGraph Runtime { get; set; }

        public System.Action OnGraphUpdate;

        public void GenerateRuntime()
        {
            Runtime = Graph.Deserialize(ProcGenSerialization.SerializationSettings, ProcGenSerialization.NodeConverter);
        }
    }
}