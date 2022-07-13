using ProcGen.Connector;
using ProcGen.Utils;
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

        private IList<int> m_SortedEvaluationTree; 

        internal RuntimeGraph()
        {
            m_SortedEvaluationTree = new List<int>();
        }


        public void Compute()
        {
            InternalEvaluate();
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
            //m_EvaluationStack.Capacity = Nodes.Length;

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

        internal void ComputeExecutionTree()
        {
            m_SortedEvaluationTree = GraphTree.ComputeExecutionTree(Nodes, TargetNode.NodeIndex);
        }

        public void SetMasterNode(BaseNode node)
        {
            TargetNode = node;
        }

        private void InternalEvaluate()
        {
            for(int i = 0; i < m_SortedEvaluationTree.Count; ++i)
            {
                Nodes[m_SortedEvaluationTree[i]].Evaluate();
            }
        }
    }
}
