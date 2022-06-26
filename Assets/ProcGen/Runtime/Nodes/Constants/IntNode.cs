namespace ProcGen.Nodes.Constants
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
}