using ProcGen.Connector;

namespace ProcGen.Nodes.Constants
{
    [ProceduralNode(0, "Float")]
    public class FloatNode : BaseNode
    {
        public float Value;

        public override void Initialize()
        {
            Outputs = this.CreateOutputs(ConnectorType.Float);
        }
        public override void Evaluate()
        {
            Outputs[0].ValueFloat = Value;
        }
    }
}