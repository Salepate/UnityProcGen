using UnityEngine;

namespace ProcGen
{
    [ProceduralNode(0, "Integer")]
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

    [ProceduralNode(0, "Float")]
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

    [ProceduralNode(0, "Vector")]
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

    [ProceduralNode(0, "Vector")]
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