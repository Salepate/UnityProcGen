namespace ProcGen
{
    [System.Serializable]
    public class RuntimeGraph
    {
        public BaseNode[] Nodes = new BaseNode[0];

        public T Query<T>(int num = 0) where T : BaseNode
        {
            int count = 0;
            for(int i = 0; i < Nodes.Length; ++i)
            {
                if (Nodes[i].GetType() == typeof(T) && count++ == num)
                    return (T) Nodes[i];
            }
            return null;
        }

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
            EvaluateNode(targetNode, recursive);
        }

        public void EvaluateNode(BaseNode targetNode, bool recursive)
        {
            if (recursive && targetNode.Inputs != null && targetNode.Inputs.Length > 0)
            {
                for (int i = 0; i < targetNode.Inputs.Length; ++i)
                { 
                    if ( targetNode.Inputs[i].IsConnectorValid())
                    {
                        EvaluateNode(targetNode.Inputs[i].Source, recursive);
                    }
                }
            }
            targetNode.Evaluate();
        }
    }
}
