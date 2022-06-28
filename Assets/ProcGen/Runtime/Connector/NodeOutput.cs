using System.Runtime.InteropServices;
using UnityEngine;

namespace ProcGen.Connector
{
    [StructLayout(LayoutKind.Explicit)]
    public struct NodeOutput
    {
        [FieldOffset(0)]
        public ConnectorType ConnectorType;
        [FieldOffset(4)]
        public float ValueFloat;
        [FieldOffset(4)]
        public int ValueInt;
        [FieldOffset(4)]
        public Vector2 ValueVector2;
        [FieldOffset(4)]
        public Vector3 ValueVector3;

        public NodeOutput(ConnectorType connectorType) : this()
        {
            ConnectorType = connectorType;
        }
    }
}