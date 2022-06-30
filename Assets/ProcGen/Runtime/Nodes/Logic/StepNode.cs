using ProcGen.Connector;

namespace ProcGen.Nodes.Logic
{
    [ProceduralNode(2, "Input", "Threshold", "Value")]
    public class StepNode : BaseNode
    {
        // in
        public const int Input = 0;
        public const int Threshold = 1;

        public override void Initialize()
        {
            Inputs = this.CreateInputs(ConnectorType.Float, ConnectorType.Float);
            Outputs = this.CreateOutputs(ConnectorType.Float);
        }
        public override void Evaluate()
        {
            if (Inputs[Input].ReadFloat() >= Inputs[Threshold].ReadFloat())
                Outputs[0].ValueFloat = 1f;
            else
                Outputs[0].ValueFloat = 0f;
        }
    }
}