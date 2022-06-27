using ProcGen;
using ProcGen.Nodes;
using ProcGenSamples;
using System.Collections.Generic;
using UnityEngine;

public class GridUpdater : MonoBehaviour
{
    public GenerativeGraphInstance Graph;
    private Mesh m_Mesh;
    private Vector3[] m_Vertices;
    private int[] m_Indices;
    private void Awake()
    {
        CreateGrid();
        GetComponent<MeshFilter>().sharedMesh = m_Mesh;

        Graph.OnGraphUpdate += OnGraphChange;
    }

    private void Start()
    {
        Graph.GenerateRuntime();
        OnGraphChange();
    }

    private void OnGraphChange()
    {
        UpdateGrid();
    }

    private void UpdateGrid()
    {
        var heightmap = Graph.Runtime.Query<HeightMapNode>();
        var tilemap = Graph.Runtime.Query<TileMapNode>();

        for (int i = 0; i < 100*100; ++i)
        {
            int y = i % 100;
            int x = i / 100;
            tilemap.m_Coordinate = new Vector2Int(x, y);
            Graph.Runtime.EvaluateNode(heightmap);
            float height = heightmap.Height;
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
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[100 * 100];
        List<int> indices = new List<int>();
        for(int i = 0; i < 100; ++i)
        {

            for(int j = 0; j < 100; ++j)
            {
                int idx = j + i * 100;

                vertices[idx] = new Vector3(i, 0, j);
            }
        }

        for(int i = 0; i < 99*99; ++i)
        {
            if (i % 100 == 99)
                continue;
            indices.Add(i);
            indices.Add(i + 1);
            indices.Add(i + 101);
            indices.Add(i + 100);
        }
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Quads, 0);
        m_Mesh = mesh;
        m_Vertices = vertices;
        m_Indices = indices.ToArray();
    }
}