using UnityEngine;

namespace Dirt.ProcGen
{
    [CreateAssetMenu(fileName = "gen_graph.asset", menuName ="Dai Castle/Generative Graph")]
    public class GenGraph : ScriptableObject
    {
        //public BaseNode[] Nodes = new BaseNode[0];
        public string SerializedGraph = "{}";
    }
}
