namespace Dirt.ProcGen
{
    [System.Serializable]
    public class RuntimeGraph
    {
        public BaseNode[] Nodes = new BaseNode[0];

        public void Initialize()
        {
            for(int i = 0; i < Nodes.Length; ++i)
            {
                Nodes[i].Initialize();
            }
        }

        public void EvaluateNode(int nodeIndex, bool recursive = true)
        {
            BaseNode targetNode = Nodes[nodeIndex];
            InternalEvaluate(targetNode, recursive);
        }

        private void InternalEvaluate(BaseNode targetNode, bool recursive)
        {
            if (recursive && targetNode.Inputs != null && targetNode.Inputs.Length > 0)
            {
                for (int i = 0; i < targetNode.Inputs.Length; ++i)
                { 
                    if ( targetNode.Inputs[i].IsConnectorValid())
                    {
                        InternalEvaluate(targetNode.Inputs[i].Source, recursive);
                    }
                }
            }
            targetNode.Evaluate();
        }
    }
}
