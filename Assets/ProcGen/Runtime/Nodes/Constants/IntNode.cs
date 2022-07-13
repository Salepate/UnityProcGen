using ProcGen.Connector;

namespace ProcGen.Nodes.Constants
{
    [ProceduralNode(0, "Integer")]
    public class IntNode : BaseNode
    {
        public int Value;

        public override void Initialize()
        {
            Outputs = this.CreateOutputs(ConnectorType.Integer);
        }
        public override void Evaluate()
        {
            Outputs[0].Value.Int = Value;
        }
    }
}