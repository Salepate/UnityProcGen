using System.Runtime.CompilerServices;
using UnityEngine;

namespace ProcGen.Connector
{
    [System.Serializable]
    public struct NodeInput
    {
        public readonly ConnectorType ConnectorType;
        public BaseNode Source { get; private set; }
        public int SourceOutputIndex { get; private set; }
        public ConnectorValue Initial;

        private NodeOutput m_SourceOutput => Source.Outputs[SourceOutputIndex];

        public NodeInput(ConnectorType cType) : this()
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
}