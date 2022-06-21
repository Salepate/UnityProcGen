using UnityEngine;

namespace Dirt.ProcGen
{
    public class IntNode : BaseNode
    {
        public int Value;

        public override void Initialize()
        {
            Outputs = NodeOutput.CreateOutputs(ConnectorType.Integer);
        }
        public override void Evaluate()
        {
            Outputs[0].ValueInt = Value;
        }
    }

    public class FloatNode : BaseNode
    {
        public float Value;

        public override void Initialize()
        {
            Outputs = NodeOutput.CreateOutputs(ConnectorType.Float);
        }
        public override void Evaluate()
        {
            Outputs[0].ValueFloat = Value;
        }
    }

    public class Vector2Node : BaseNode
    {
        public Vector2 Value;

        public override void Initialize()
        {
            Outputs = NodeOutput.CreateOutputs(ConnectorType.Vector2);
        }
        public override void Evaluate()
        {
            Outputs[0].ValueVector2 = Value;
        }
    }


    public class Vector3Node : BaseNode
    {
        public Vector3 Value;

        public override void Initialize()
        {
            Outputs = NodeOutput.CreateOutputs(ConnectorType.Vector3);
        }
        public override void Evaluate()
        {
            Outputs[0].ValueVector3 = Value;
        }
    }

}