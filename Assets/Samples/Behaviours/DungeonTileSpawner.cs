using ProcGen;
using ProcGen.Nodes;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenSamples
{
    public class DungeonTileSpawner : MonoBehaviour
    {
        public GenerativeGraphInstance Graph;
        public int TilesPerSide = 50;
        // cache
        private List<GameObject> m_Tiles;
        // graph queries
        private TileMapNode m_Tilemap;

        private void Awake()
        {
            AllocateTiles();
        }
        private void Start()
        {
            OnGraphChange();
            Graph.OnGraphUpdate += OnGraphChange; // only invoked in editor
        }

        private void OnEnable()
        {
            Graph.GenerateRuntime();

        }
        private void OnDisable()
        {
            Graph.Clear();
        }

        private void AllocateTiles()
        {
            GameObject rootObj = new GameObject("DungeonRoot");
            m_Tiles = new List<GameObject>();
            Quaternion baseRot = Quaternion.Euler(90f, 0f, 0f);
            for (int i = 0; i < TilesPerSide * TilesPerSide; ++i)
            {
                int x = i % TilesPerSide;
                int y = i / TilesPerSide;
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tile.transform.SetParent(rootObj.transform);
                tile.transform.localRotation = baseRot;
                tile.transform.localPosition = new Vector3(x - (TilesPerSide) / 2f, 0f, y - (TilesPerSide) / 2f);
                m_Tiles.Add(tile);
            }
        }

        private void OnGraphChange()
        {
            m_Tilemap = Graph.Runtime.Query<TileMapNode>(); // just in case it was destroyed/recreated
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            for(int i = 0; i < m_Tiles.Count; ++i)
            {
                int x = i % TilesPerSide;
                int y = i / TilesPerSide;
                GameObject tile = m_Tiles[i];
                m_Tilemap.Coordinate = new Vector2Int(x,y);
                Graph.Runtime.Compute();
                int tileInt = Graph.Buffer.ReadValueInt(0);
                tile.transform.localScale = Vector3.one * tileInt * Graph.Buffer.ReadValueFloat(1); 
            }
        }
    }
}
