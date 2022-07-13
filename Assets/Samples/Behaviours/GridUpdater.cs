using ProcGen;
using ProcGen.Nodes;
using ProcGenSamples;
using System.Collections.Generic;
using UnityEngine;

public class GridUpdater : MonoBehaviour
{
    public float Scale = 1f;
    public int TileCount { get; private set; }
    public int GridSize = 100;
    public int TileOffset => GridSize - 1;
    public GenerativeGraphInstance Graph;
    private Mesh m_Mesh;
    private Vector3[] m_Vertices;
    private int[] m_Indices;

    private TimeSampler m_Time;
    private void Awake()
    {
        CreateGrid();
        GetComponent<MeshFilter>().sharedMesh = m_Mesh;

        Graph.OnGraphUpdate += OnGraphChange;
    }

    private void OnEnable()
    {
        Graph.GenerateRuntime();
        OnGraphChange();
    }

    private void OnDisable()
    {
        Graph.Clear();
    }

    private void Update()
    {
        if (m_Time != null)
            m_Time.CacheTimeValue();

        UpdateGrid();
    }

    private void OnGraphChange()
    {
        m_Time = Graph.Runtime.Query<TimeSampler>();
        m_Time.ManualTimeUpdate = true;
        UpdateGrid();
    }

    private void UpdateGrid()
    {
        var tilemap = Graph.Runtime.Query<TileMapNode>();

        for (int i = 0; i < TileCount; ++i)
        {
            int y = i % GridSize;
            int x = i / GridSize;
            tilemap.Coordinate = new Vector2Int(x, y);
            Graph.Runtime.Compute();
            float height = Graph.Buffer.ReadValueFloat(0);
            ref Vector3 p = ref m_Vertices[i];
            p.y = height;
        }
        m_Mesh.SetVertices(m_Vertices);
        m_Mesh.SetIndices(m_Indices, MeshTopology.Quads, 0);
        m_Mesh.RecalculateBounds();
        m_Mesh.RecalculateNormals();
    }

    private void CreateGrid()
    {
        TileCount = GridSize * GridSize;

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[TileCount];
        List<int> indices = new List<int>();
        for(int i = 0; i < GridSize; ++i)
        {
            for(int j = 0; j < GridSize; ++j)
            {
                int idx = j + i * GridSize;

                float x = (float)i / TileOffset * Scale;
                float z = (float)j / TileOffset * Scale;

                vertices[idx] = new Vector3(x, 0, z);
            }
        }

        for(int i = 0; i < (TileOffset*GridSize); ++i)
        {
            if (i % GridSize == TileOffset)
                continue;
            indices.Add(i);
            indices.Add(i + 1);
            indices.Add(i + GridSize + 1);
            indices.Add(i + GridSize);
        }
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Quads, 0);
        m_Mesh = mesh;
        m_Vertices = vertices;
        m_Indices = indices.ToArray();
    }
}