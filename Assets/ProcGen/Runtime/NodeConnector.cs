using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ProcGen
{
    [System.Serializable]
    public struct NodeConnector
    {

        public readonly ConnectorType ConnectorType;
        public BaseNode Source { get; private set; }
        public int SourceOutputIndex { get; private set; }
        public ConnectorValue Initial;

        private NodeOutput m_SourceOutput => Source.Outputs[SourceOutputIndex];

        public NodeConnector(ConnectorType cType) : this()
        {
            ConnectorType = cType;
            Source = null;
            SourceOutputIndex = -1;
            Initial = new ConnectorValue();
        }

        public void Connect(BaseNode sourceNode, int outputIndex)
        {
            Source = sourceNode;
            SourceOutputIndex = outputIndex;
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


        public int ReadInteger()
        {
            if (IsConnectorValid())
            {
                return ConnectorHelper.ConvertInt(m_SourceOutput);
            }
            return Initial.InitialValueInt;
        }

        public float ReadFloat()
        {
            if (IsConnectorValid())
            {
                return ConnectorHelper.ConvertFloat(m_SourceOutput);
            }
            return Initial.InitialValueFloat;
        }
        public Vector2 ReadVector2()
        {
            if (IsConnectorValid())
            {
                return m_SourceOutput.ValueVector2;
            }
            return Initial.InitialValueVector2;
        }
        public Vector3 ReadVector3()
        {
            if (IsConnectorValid())
            {
                return m_SourceOutput.ValueVector3;
            }
            return Initial.InitialValueVector3;
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
        public Vector3 ValueVector3;

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

            if (c2 == ConnectorType.SourceType) // passthrough for math ops
                return true;

            if (c1 == ConnectorType.Vector2 && c2 == ConnectorType.Vector3)
                return true;

            if (c1 == ConnectorType.Integer && c2 == ConnectorType.Float)
                return true;

            return false;
        }

        public static Type GetAssociatedType(ConnectorType connectorType)
        {
            switch (connectorType)
            {
                case ConnectorType.Integer: return typeof(int);
                case ConnectorType.Float: return typeof(float);
                case ConnectorType.Vector2: return typeof(Vector2);
                case ConnectorType.Vector3: return typeof(Vector3);
            }
            return typeof(object);
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
        Vector3,
        SourceType
    }
}