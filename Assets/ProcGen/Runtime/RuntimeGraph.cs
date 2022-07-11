using ProcGen.Connector;
using System.Collections.Generic;

namespace ProcGen
{
    /// <summary>
    /// Runtime instance of a GenerativeGraphInstance
    /// </summary>
    public class RuntimeGraph
    {
        public BaseNode[] Nodes = new BaseNode[0];

        public BaseNode TargetNode { get; private set; }

        private List<int> m_EvaluationStack; 

        internal RuntimeGraph()
        {
            m_EvaluationStack = new List<int>();
        }


        public void Compute()
        {
            InternalEvaluate(TargetNode.NodeIndex);
        }

        /// Legacy

        /// <summary>
        /// Fetch an instanciated node by type
        /// </summary>
        /// <typeparam name="T">Node type</typeparam>
        /// <param name="num">node offset (in case of multiple instances of a same type)</param>
        /// <returns>the {num} node found in the array (unsorted), null if the node wasnt found</returns>
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

        internal void Initialize(int mainNodeIndex)
        {
            m_EvaluationStack.Capacity = Nodes.Length;

            if (mainNodeIndex != -1 && Nodes[mainNodeIndex] is IMasterNode )
            {
                TargetNode = Nodes[mainNodeIndex];
            }

            for (int i = 0; i < Nodes.Length; ++i)
            {
                Nodes[i].Initialize();
                Nodes[i].SetIndex(i);
            }
        }

        public void SetMasterNode(BaseNode node)
        {
            TargetNode = node;
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
                    ref NodeInput connector = ref currentNode.Inputs[i];
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
