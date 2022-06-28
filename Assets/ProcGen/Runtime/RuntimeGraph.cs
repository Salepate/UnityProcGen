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

        private List<int> m_EvaluationStack; 

        internal RuntimeGraph()
        {
            m_EvaluationStack = new List<int>();
        }

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

        /// <summary>
        /// Evaluates a specific node from the graph and its subtree
        /// </summary>
        /// <param name="nodeIndex">node index in array</param>
        public void EvaluateNode(int nodeIndex)
        {
            if (m_EvaluationStack.Count > 0)
                throw new System.Exception("Graph failed to evaluate properly");

            InternalEvaluate(nodeIndex);
        }
        /// <summary>
        /// Evaluates a referenced node from the graph and its subtree
        /// throws an exception if the node is not included in the graph.
        /// </summary>
        /// <param name="node">node reference belonging to the graph</param>
        public void EvaluateNode(BaseNode node)
        {
            if (node.NodeIndex == -1 || System.Array.IndexOf(Nodes, node) == -1)
                throw new System.Exception($"Node {node.GetType().Name} does not belong to current graph");

            if (m_EvaluationStack.Count > 0)
                throw new System.Exception("Graph failed to evaluate properly");

            InternalEvaluate(node.NodeIndex);
        }

        internal void Initialize()
        {
            m_EvaluationStack.Capacity = Nodes.Length;

            for (int i = 0; i < Nodes.Length; ++i)
            {
                Nodes[i].Initialize();
                Nodes[i].SetIndex(i);
            }
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
