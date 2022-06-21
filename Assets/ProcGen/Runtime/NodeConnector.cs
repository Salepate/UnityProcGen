using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Dirt.ProcGen
{
    public struct NodeConnector
    {
        public readonly ConnectorType ConnectorType;
        public BaseNode Source { get; private set; }
        private int m_SourceOutputIndex;

        private NodeOutput m_SourceOutput => Source.Outputs[m_SourceOutputIndex];

        public NodeConnector(ConnectorType cType)
        {
            ConnectorType = cType;
            Source = null;
            m_SourceOutputIndex = -1;
        }

        public void Connect(BaseNode sourceNode, int outputIndex)
        {
            Source = sourceNode;
            m_SourceOutputIndex = outputIndex;
        }

        public static NodeConnector[] CreateInputs(params ConnectorType[] types) 
        {
            NodeConnector[] res = new NodeConnector[types.Length];
            for(int i = 0; i < types.Length; ++i)
            {
                res[i] = new NodeConnector(types[i]);
            }
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsConnectorValid()
        {
            bool sourceExists = Source != null && Source.Outputs != null && m_SourceOutputIndex < Source.Outputs.Length;
            return sourceExists && ConnectorHelper.CanConvert(m_SourceOutput.ConnectorType, ConnectorType);
        }


        public int ReadInteger()
        {
            if (IsConnectorValid())
            {
                return ConnectorHelper.ConvertInt(m_SourceOutput);
            }
            return 0;
        }

        public float ReadFloat()
        {
            if (IsConnectorValid())
            {
                return ConnectorHelper.ConvertFloat(m_SourceOutput);
            }
            return 0f;
        }
        public Vector2 ReadVector2()
        {
            if (IsConnectorValid())
            {
                return m_SourceOutput.ValueVector2;
            }
            return Vector2.zero;
        }
        public Vector3 ReadVector3()
        {
            if (IsConnectorValid())
            {
                return m_SourceOutput.ValueVector3;
            }
            return Vector3.zero;
        }
    }

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
        public Vector2 ValueVector3;

        public NodeOutput(ConnectorType connectorType) : this()
        {
            ConnectorType = connectorType;
        }

        public static NodeOutput[] CreateOutputs(params ConnectorType[] types)
        {
            NodeOutput[] outputs = new NodeOutput[types.Length];
            for (int i = 0; i < types.Length; ++i)
                outputs[i] = new NodeOutput(types[i]);
            return outputs;
        }
    }


    public static class ConnectorHelper
    {
        public static bool NeedsConvert(ConnectorType c1, ConnectorType c2)
        {
            return c1 != c2;
        }

        public static bool CanConvert(ConnectorType c1, ConnectorType c2)
        {
            if (c1 == c2)
                return true;

            if ( c2 < c1 )
            {
                ConnectorType tmp = c1;
                c1 = c2;
                c2 = tmp;
            }

            if (c1 == ConnectorType.Integer && c2 == ConnectorType.Float)
                return true;

            return false;
        }

        internal static int ConvertInt(NodeOutput sourceOutput)
        {
            if (sourceOutput.ConnectorType == ConnectorType.Float)
                return (int)sourceOutput.ValueFloat;
            return sourceOutput.ValueInt;
        }
        internal static float ConvertFloat(NodeOutput sourceOutput)
        {
            if (sourceOutput.ConnectorType == ConnectorType.Integer)
                return sourceOutput.ValueInt;
            return sourceOutput.ValueFloat;
        }
    }

    public enum ConnectorType : ushort
    {
        Integer,
        Float,
        Vector2,
        Vector3
    }
}