using ProcGen.Buffer;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ProcGen.Connector
{
    [System.Serializable]
    public struct NodeInput
    {
        public readonly ConnectorType ConnectorType;
        public BaseNode Source { get; private set; }
        public int SourceOutputIndex { get; private set; }
        public ValueBuffer Initial;

        private NodeOutput m_SourceOutput => Source.Outputs[SourceOutputIndex];

        public NodeInput(ConnectorType cType) : this()
        {
            ConnectorType = cType;
            Source = null;
            SourceOutputIndex = -1;
            Initial = new ValueBuffer();
        }

        public void Connect(BaseNode sourceNode, int outputIndex)
        {
            Source = sourceNode;
            SourceOutputIndex = outputIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsConnectorValid()
        {
            bool sourceExists = Source != null && Source.Outputs != null && SourceOutputIndex < Source.Outputs.Length;
            return sourceExists && ConnectorHelper.CanConvert(m_SourceOutput.ConnectorType, ConnectorType);
        }

        public bool TryGetAttachedOutput(out NodeOutput output)
        {
            output = default;
            if (IsConnectorValid())
            {
                output = Source.Outputs[SourceOutputIndex];
                return true;
            }
            return false;
        }

        public bool ReadBytesInto(IntPtr addrPointer, bool deleteData, byte[] destination, int offset)
        {
            int size = ConnectorHelper.GetDataSize(ConnectorType);
            // naive
            if ( IsConnectorValid())
            {
                Marshal.StructureToPtr(m_SourceOutput.Value, addrPointer, deleteData);
                Marshal.Copy(addrPointer, destination, offset, size);
                return true;
            }
            return false;
        }
        public int ReadInteger()
        {
            if (IsConnectorValid())
            {
                return ConnectorHelper.ConvertInt(m_SourceOutput);
            }
            return Initial.Int;
        }

        public float ReadFloat()
        {
            if (IsConnectorValid())
            {
                return ConnectorHelper.ConvertFloat(m_SourceOutput);
            }
            return Initial.Float;
        }
        public Vector2 ReadVector2()
        {
            if (IsConnectorValid())
            {
                return m_SourceOutput.Value.Vec2;
            }
            return Initial.Vec2;
        }
        public Vector3 ReadVector3()
        {
            if (IsConnectorValid())
            {
                return m_SourceOutput.Value.Vec3;
            }
            return Initial.Vec3;
        }
    }
}