using NUnit.Framework;
using ProcGen.Buffer;
using System;
using System.Runtime.InteropServices;

namespace ProcGen.Tests
{
    public static class BufferTests
    {
        [Test]
        public static void TestMarshalCopy()
        {
            const int testValue = 255;
            const int size = 4;

            ValueBuffer valueBuffer = new ValueBuffer();
            valueBuffer.Int = testValue;
            byte[] copy = new byte[size];
            IntPtr ptr;
            // naive
            int marshalSize = Marshal.SizeOf(valueBuffer);
            ptr = Marshal.AllocHGlobal(marshalSize);
            Marshal.StructureToPtr(valueBuffer, ptr, false);
            Marshal.Copy(ptr, copy, 0, size);
            Marshal.FreeHGlobal(ptr);

            byte[] original = new byte[4];
            original[0] = (byte)testValue;
            original[1] = (byte)(testValue >> 8);
            original[2] = (byte)(testValue >> 16);
            original[3] = (byte)(testValue >> 24);


            for(int i = 0; i < size; ++i)
            {
                Assert.AreEqual(original[i], copy[i]);
            }

        }
    }
}