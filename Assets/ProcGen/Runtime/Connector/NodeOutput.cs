using ProcGen.Buffer;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ProcGen.Connector
{
    public struct NodeOutput
    {
        public ConnectorType ConnectorType;
        public ValueBuffer Value;

        public NodeOutput(ConnectorType connectorType) : this()
        {
            ConnectorType = connectorType;
        }

        internal void Write(ref NodeInput input, ConnectorType type, bool rewriteOutputType = false)
        {
            if (rewriteOutputType)
                ConnectorType = type;

            switch (type)
            {
                case ConnectorType.Integer: Value.Write(input.ReadInteger());
                    break;
                case ConnectorType.Float: Value.Write(input.ReadFloat());
                    break;
                case ConnectorType.Vector2: Value.Write(input.ReadVector2());
                    break;
                case ConnectorType.Vector3: Value.Write(input.ReadVector3());
                    break;
            }
        }
    }
}