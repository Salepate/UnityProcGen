using System.Runtime.CompilerServices;
using UnityEngine;

namespace ProcGen.Connector
{
    using Type = System.Type;

    public static class ConnectorHelper
    {
        public static NodeInput[] CreateInputs(this BaseNode node, params ConnectorType[] types)
        {
            NodeInput[] res = new NodeInput[types.Length];
            for (int i = 0; i < types.Length; ++i)
            {
                res[i] = new NodeInput(types[i]);
            }
            return res;
        }

        public static NodeOutput[] CreateOutputs(this BaseNode node, params ConnectorType[] types)
        {
            NodeOutput[] outputs = new NodeOutput[types.Length];
            for (int i = 0; i < types.Length; ++i)
                outputs[i] = new NodeOutput(types[i]);
            return outputs;
        }

        public static bool CanConvert(ConnectorType c1, ConnectorType c2)
        {
            if (c1 == c2)
                return true;

            if (c2 < c1)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetDataSize(ConnectorType connectorType)
        {
            const int compSize = 4;
            switch (connectorType)
            {
                default:
                case ConnectorType.Integer:
                case ConnectorType.Float:
                    return compSize;
                case ConnectorType.Vector2:
                    return compSize * 2;
                case ConnectorType.Vector3:
                    return compSize * 3;
                case ConnectorType.SourceType:
                    return compSize * 4;
            }
        }

        internal static int ConvertInt(NodeOutput sourceOutput)
        {
            if (sourceOutput.ConnectorType == ConnectorType.Float)
                return (int)sourceOutput.Value.Float;
            return sourceOutput.Value.Int;
        }
        internal static float ConvertFloat(NodeOutput sourceOutput)
        {
            if (sourceOutput.ConnectorType == ConnectorType.Integer)
                return sourceOutput.Value.Int;
            return sourceOutput.Value.Float;
        }
    }
}