using System.Collections.Generic;

namespace ProcGen
{
    [System.Serializable]
    public class RuntimeGraph
    {
        public BaseNode[] Nodes = new BaseNode[0];

        private List<int> m_EvaluationStack; 

        public RuntimeGraph()
        {
            m_EvaluationStack = new List<int>();
        }

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
            m_EvaluationStack.Capacity = Nodes.Length;

            for (int i = 0; i < Nodes.Length; ++i)
            {
                Nodes[i].Initialize();
                Nodes[i].SetIndex(i);
            }
        }

        public void EvaluateNode(int nodeIndex)
        {
            if (m_EvaluationStack.Count > 0)
                throw new System.Exception("Graph failed to evaluate properly");

            InternalEvaluate(nodeIndex);
        }

        public void EvaluateNode(BaseNode node)
        {
            if ( node.NodeIndex == -1 || System.Array.IndexOf(Nodes, node) == -1)
                throw new System.Exception($"Node {node.GetType().Name} does not belong to current graph");

            if (m_EvaluationStack.Count > 0)
                throw new System.Exception("Graph failed to evaluate properly");

            InternalEvaluate(node.NodeIndex);
        }

        private void InternalEvaluate(int targetNode)
        {
            int offset = 0;
            m_EvaluationStack.Add(targetNode);

            // breadthfirst traversal
            do
            {
                int nodeIndex = m_EvaluationStack[offset];
                BaseNode currentNode = Nodes[nodeIndex];

                for (int i = 0; i < currentNode.Inputs.Length; ++i)
                {
                    ref NodeConnector connector = ref currentNode.Inputs[i];
                    BaseNode sourceNode = connector.Source;

                    if ( connector.IsConnectorValid() && !m_EvaluationStack.Contains(sourceNode.NodeIndex))
                    {
                        m_EvaluationStack.Add(sourceNode.NodeIndex);
                    }
                }

            } while (++offset < m_EvaluationStack.Count);

            // stack pop
            for(int i = m_EvaluationStack.Count - 1; i >= 0; --i)
            {
                int nodeIndex = m_EvaluationStack[i];
                Nodes[nodeIndex].Evaluate();
            }
            m_EvaluationStack.Clear();
        }
    }
}
