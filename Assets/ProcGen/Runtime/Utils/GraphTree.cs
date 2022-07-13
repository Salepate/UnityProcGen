using ProcGen.Connector;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGen.Utils
{
    public static class GraphTree
    {
        public static IList<int> ComputeExecutionTree(BaseNode[] directedGraph, int initialNode)
        {
            Stack<int> stack = new Stack<int>();

            int[] visitOrder = new int[directedGraph.Length];
            for (int i = 0; i < visitOrder.Length; ++i)
                visitOrder[i] = -1;

            Dictionary<int, int> nodeWeight = new Dictionary<int, int>();
            HashSet<int> visited = new HashSet<int>();
            List<int> computeOrder = new List<int>();
            nodeWeight.Add(initialNode, 0);
            stack.Push(initialNode);
            visited.Add(initialNode);
            int visitIndex = 0;
            while(stack.Count > 0 )
            {
                int nodeIndex = stack.Pop();
                BaseNode node = directedGraph[nodeIndex];
                int weight = nodeWeight[nodeIndex];
                visitOrder[nodeIndex] = visitIndex++;
                computeOrder.Add(nodeIndex);

                for (int i = 0; i < node.Inputs.Length; ++i)
                {
                    if ( node.Inputs[i].IsConnectorValid())
                    {
                        int sourceIndex = node.Inputs[i].Source.NodeIndex;
                        if (!nodeWeight.ContainsKey(sourceIndex))
                            nodeWeight[sourceIndex] = weight + 1; 
                        else
                            nodeWeight[sourceIndex] = Mathf.Max(weight + 1, nodeWeight[sourceIndex]);

                        if (!visited.Contains(sourceIndex))
                        {
                            stack.Push(sourceIndex);
                            visited.Add(sourceIndex);
                        }
                    }
                }
            }
            computeOrder.Sort((a, b) =>
            {
                int weightDt = nodeWeight[b] - nodeWeight[a];
                if (weightDt != 0)
                    return weightDt;

                return visitOrder[b] - visitOrder[a];
            });

            return computeOrder;
        }
    }
}