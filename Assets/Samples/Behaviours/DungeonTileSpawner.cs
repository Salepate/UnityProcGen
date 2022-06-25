using ProcGen;
using ProcGen.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenSamples
{
    public class DungeonTileSpawner : MonoBehaviour
    {
        public GenerativeGraphInstance Graph;
        private TileMapNode m_Tilemap;
        private DungeonTileNode m_DungeonTile;

        public int TileSize = 50;
        private int m_X;
        private int m_Y;

        private List<GameObject> m_Tiles;

        void Start()
        {
            Graph.GenerateRuntime();
            m_Tilemap = Graph.Runtime.Query<TileMapNode>();
            m_DungeonTile = Graph.Runtime.Query<DungeonTileNode>();
            m_Tiles = new List<GameObject>();
            UpdateGrid(0, 0);
            Graph.OnGraphUpdate += OnGraphChange;
        }

        private void OnGraphChange()
        {
            UpdateGrid(m_X, m_Y); // refresh
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("<", GUILayout.Width(20f)))
                UpdateGrid(m_X - 1, m_Y);
            GUILayout.Label($"X: {m_X}", GUILayout.Width(50f));
            if (GUILayout.Button(">", GUILayout.Width(20f)))
                UpdateGrid(m_X + 1, m_Y);

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("<", GUILayout.Width(20f)))
                UpdateGrid(m_X, m_Y-1);
            GUILayout.Label($"Y: {m_Y}", GUILayout.Width(50f));
            if (GUILayout.Button(">", GUILayout.Width(20f)))
                UpdateGrid(m_X, m_Y+1);

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void UpdateGrid(int x, int y)
        {
            m_X = x;
            m_Y = y;

            for (int i = 0; i < m_Tiles.Count; ++i)
                Destroy(m_Tiles[i]);

            m_Tiles.Clear();

            for (int i = 0; i < TileSize; ++i)
            {
                for(int j = 0; j < TileSize; ++j)
                {
                    m_Tilemap.m_Coordinate = new Vector2Int(x * TileSize + i, y * TileSize + j);
                    Graph.Runtime.EvaluateNode(m_DungeonTile);
                    if ( m_DungeonTile.IsTile )
                    {
                        var prim = GameObject.CreatePrimitive(PrimitiveType.Quad);
                        prim.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                        prim.transform.position = new Vector3(i - (TileSize) / 2f, 0f, j - (TileSize)/2f);
                        m_Tiles.Add(prim);
                    }
                }
            }
        }
    }
}
