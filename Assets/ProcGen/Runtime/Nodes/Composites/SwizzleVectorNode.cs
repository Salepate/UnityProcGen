using UnityEngine;

namespace ProcGen.Nodes.Composites
{
    [ProceduralNode(1, "Vector", "Vector")]
    public class SwizzleVectorNode : BaseNode
    {
        public enum Axis
        {
            X,
            Y,
            Z
        }

        public Axis X = Axis.X;
        public Axis Y = Axis.Y;
        public Axis Z = Axis.Z;

        public override void Initialize()
        {
            Inputs = NodeConnector.CreateInputs(ConnectorType.Vector3);
            Outputs = NodeOutput.CreateOutputs(ConnectorType.Vector3);
        }
        public override void Evaluate()
        {
            Vector3 input = Inputs[0].ReadVector3();
            Outputs[0].ValueVector3 = new Vector3()
            {
                x = GetAxisValue(ref input, X),
                y = GetAxisValue(ref input, Y),
                z = GetAxisValue(ref input, Z)
            };
        }

        private static float GetAxisValue(ref Vector3 value, Axis axis)
        {
            switch (axis)
            {
                default:
                case Axis.X: return value.x;
                case Axis.Y: return value.y;
                case Axis.Z: return value.z;
            }
        }
    }
}