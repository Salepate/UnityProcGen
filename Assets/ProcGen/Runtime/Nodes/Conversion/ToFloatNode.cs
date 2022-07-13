using ProcGen.Connector;

namespace ProcGen.Nodes
{
    [ProceduralNode(strict: true, 1, "Int", "Float")]
    public class ToFloatNode : BaseNode
    {
        public override void Initialize()
        {
            Inputs = this.CreateInputs(ConnectorType.Integer);
            Outputs = this.CreateOutputs(ConnectorType.Float);
        }

        public override void Evaluate()
        {
            Outputs[0].Value.Float = Inputs[0].ReadInteger();
        }
    }
}