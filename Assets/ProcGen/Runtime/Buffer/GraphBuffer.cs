using ProcGen.Connector;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ProcGen.Buffer
{
    public struct GraphBuffer
    {
        private float[] m_FloatBuffer; // buffer used to blockcopy
        private byte[] m_Block;
        private int m_Offset;
        private int[] m_SubtypesOffsets;
        private IntPtr m_ValueBuffer;
        private bool m_BufferWriteState;

        public GraphBuffer(byte[] block, int startOffset, IList<ConnectorType> subtypes)
        {
            m_Block = block;
            m_Offset = startOffset;
            m_FloatBuffer = new float[3];
            m_SubtypesOffsets = new int[subtypes.Count];
            int off = 0;
            for(int i = 0; i < subtypes.Count; ++i)
            {
                m_SubtypesOffsets[i] = off;
                off += ConnectorHelper.GetDataSize(subtypes[i]);
            }

            m_ValueBuffer = Marshal.AllocHGlobal(Marshal.SizeOf<ValueBuffer>());
            m_BufferWriteState = false;
        }

        internal void WriteBytes(ref NodeInput input, int elementIndex)
        {
            input.ReadBytesInto(m_ValueBuffer, m_BufferWriteState, m_Block, GetElementOffset(elementIndex));

            if ( !m_BufferWriteState)
                m_BufferWriteState = true;
        }

        public void Seek(int globalOffset)
        {
            m_Offset = globalOffset;
        }

        public static int ComputeBufferSize(int elementCount, IList<ConnectorType> list)
        {
            int size = 0;
            for (int i = 0; i < list.Count; ++i)
            {
                size += ConnectorHelper.GetDataSize(list[i]);
            }
            return size;
        }

        public int ReadValueInt(int index)
        {
            return System.BitConverter.ToInt32(m_Block, GetElementOffset(index));
        }

        public float ReadValueFloat(int index)
        {
            return System.BitConverter.ToSingle(m_Block, GetElementOffset(index));
        }

        public Vector3 ReadValueV3(int index)
        {
            int off = GetElementOffset(index);
            return new Vector3()
            {
                x = GetElementOffset(index),
                y = GetElementOffset(index + 4),
                z = GetElementOffset(index + 8)
            };
        }

        //public void WriteValueInt(int index, int value)
        //{
        //    int off = GetElementOffset(index);
        //    m_Block[off + 0] = (byte)value;
        //    m_Block[off + 1] = (byte)(value >> 8);
        //    m_Block[off + 2] = (byte)(value >> 16);
        //    m_Block[off + 3] = (byte)(value >> 24);
        //}

        //public void WriteValueFloat(int index, float value)
        //{
        //    m_FloatBuffer[0] = value;
        //    System.Buffer.BlockCopy(m_FloatBuffer, 0, m_Block, GetElementOffset(index), 1);
        //}

        //public void WriteValueV3(int index, Vector3 value)
        //{
        //    m_FloatBuffer[0] = value.x;
        //    m_FloatBuffer[1] = value.y;
        //    m_FloatBuffer[2] = value.z;
        //    System.Buffer.BlockCopy(m_FloatBuffer, 0, m_Block, GetElementOffset(index), 3);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetElementOffset(int index)
        {
            return m_Offset + m_SubtypesOffsets[index];
        }

        public void FreePointer()
        {
            Marshal.FreeHGlobal(m_ValueBuffer);
            m_ValueBuffer = IntPtr.Zero;
        }
    }
}