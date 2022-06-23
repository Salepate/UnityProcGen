using Dirt.ProcGen;
using ProcGen.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenSamples
{
    public class DungeonTileSpawner : MonoBehaviour
    {
        // Start is called before the first frame update
        public GenerativeGraph Graph;
        private RuntimeGraph m_GraphInstance;
        private TileMapNode m_Tilemap;
        private DungeonTileNode m_DungeonTile;

        public int TileSize = 50;
        private int m_X;
        private int m_Y;

        private List<GameObject> m_Tiles;

        void Start()
        {
            m_GraphInstance = Graph.Deserialize(ProcGenSerialization.SerializationSettings, ProcGenSerialization.NodeConverter);
            m_Tilemap = m_GraphInstance.Query<TileMapNode>();
            m_DungeonTile = m_GraphInstance.Query<DungeonTileNode>();
            m_Tiles = new List<GameObject>();
            UpdateGrid(0, 0);
        }

        // Update is called once per frame
        void Update()
        {
        
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
                    m_GraphInstance.EvaluateNode(m_DungeonTile, true);
                    if ( m_DungeonTile.IsTile )
                    {
                        var prim = GameObject.CreatePrimitive(PrimitiveType.Quad);
                        prim.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                        prim.transform.position = new Vector3((x * TileSize + i), 0f, (y * TileSize + j));
                        m_Tiles.Add(prim);
                    }
                }
            }
        }
    }
}
