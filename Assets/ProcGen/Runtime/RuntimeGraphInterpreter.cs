using Dirt.ProcGen.Nodes;
using System.Collections.Generic;
using UnityEngine;

namespace Dirt.ProcGen
{
    public class RuntimeGraphInterpreter : MonoBehaviour
    {
        public RuntimeGraph Graph;

        public TileMapNode Tilemap;

        private void Awake()
        {
            List<BaseNode> nodes = new List<BaseNode>();
            Graph = new RuntimeGraph();

            int perlinIndex, scaleIndex, coordIndex, createVecIndex;

            // Create a perlin generator
            UnityPerlinNode perlin = new UnityPerlinNode();
            perlinIndex = nodes.Count;
            nodes.Add(perlin);

            // add constant
            scaleIndex = nodes.Count;
            nodes.Add(new Vector2Node() { Value = new Vector2(0.2f, 0.2f) });

            // add tile map
            coordIndex = nodes.Count;
            Tilemap = new TileMapNode();
            nodes.Add(Tilemap);

            // add vector2 composite
            createVecIndex = nodes.Count;
            nodes.Add(new CreateVector2Node());

            // initialize connectors
            for (int i = 0; i < nodes.Count; ++i)
                nodes[i].Initialize();

            // link composite to tilemap
            nodes[createVecIndex].Inputs[CreateVector2Node.X].Connect(nodes[coordIndex], 0);
            nodes[createVecIndex].Inputs[CreateVector2Node.Y].Connect(nodes[coordIndex], 1);

            // link perlin to Constant & Composite
            nodes[perlinIndex].Inputs[0].Connect(nodes[scaleIndex], 0);
            nodes[perlinIndex].Inputs[1].Connect(nodes[createVecIndex], 0);

            Graph.Nodes = nodes.ToArray();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Evaluate"))
                Evaluate();
        }

        public void Evaluate()
        {
            Graph.EvaluateNode(0, true); // Evaluate Perlin
            UnityPerlinNode perlin = (UnityPerlinNode) Graph.Nodes[0];
            Debug.Log($"Perlin: {perlin.Outputs[0].ValueFloat}");
        }
    }
}