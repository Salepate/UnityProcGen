using ProcGen.Connector;

namespace ProcGen.Nodes
{
    [ProceduralNode(strict: true, 1, "Float", "Int")]
    public class ToIntNode : BaseNode
    {
        public override void Initialize()
        {
            Inputs = this.CreateInputs(ConnectorType.Float);
            Outputs = this.CreateOutputs(ConnectorType.Integer);
        }

        public override void Evaluate()
        {
            Outputs[0].Value.Int = (int) Inputs[0].ReadFloat();
        }
    }
}