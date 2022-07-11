using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ProcGen.Buffer
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ValueBuffer
    {
        [FieldOffset(0)]
        public int Int;
        [FieldOffset(0)]
        public float Float;
        [FieldOffset(0)]
        public Vector2 Vec2;
        [FieldOffset(0)]
        public Vector3 Vec3;
        [FieldOffset(0)]
        public Vector4 Vec4;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(float value) { Float = value; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int value) { Int = value; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(Vector2 vector) { Vec2 = vector; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(Vector3 vector) { Vec3 = vector; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(Vector4 vector) { Vec4 = vector; }
    }
}