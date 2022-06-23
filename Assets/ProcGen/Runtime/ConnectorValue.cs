using System.Runtime.InteropServices;
using UnityEngine;

namespace Dirt.ProcGen
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ConnectorValue
    {
        [FieldOffset(0)]
        public int InitialValueInt;
        [FieldOffset(0)]
        public float InitialValueFloat;
        [FieldOffset(0)]
        public Vector2 InitialValueVector2;
        [FieldOffset(0)]
        public Vector3 InitialValueVector3;
        [FieldOffset(0)]
        public Vector4 InitialValueVector4;
    }
}
