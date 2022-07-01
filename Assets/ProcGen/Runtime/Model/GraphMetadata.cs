using UnityEngine;

namespace ProcGen.Model
{
    [System.Serializable]
    public struct GraphMetadata
    {
        public GroupMetadata[] Groups;
        public NodeMetadata[] Nodes;
    }

    [System.Serializable]
    public struct GroupMetadata
    {
        public string Title;
        public Color Color;
    }

    [System.Serializable]
    public struct NodeMetadata
    {
        public Rect Position;
        public int GroupIndex;
    }
}
