using ProcGen.Connector;

namespace ProcGen.Nodes.Logic
{
    [ProceduralNode(3, "Condition", "If True", "If False", "Branch")]
    public class BranchNode : BaseNode
    {
        // in
        public const int Condition = 0;
        public const int TrueBranch = 1;
        public const int FalseBranch = 2;
        public override void Initialize()
        {
            Inputs = this.CreateInputs(ConnectorType.Integer, ConnectorType.SourceType, ConnectorType.SourceType);
            Outputs = this.CreateOutputs(ConnectorType.SourceType);
        }
        public override void Evaluate()
        {
            NodeOutput source;
            ConnectorType outType = ConnectorType.SourceType;
            int inputIndex = (Inputs[Condition].ReadInteger() > 0) ? TrueBranch : FalseBranch;
            ref NodeInput input = ref Inputs[inputIndex];
            //todo display error in case true/false type mismatch
            if (input.TryGetAttachedOutput(out source))
                outType = source.ConnectorType;

            Outputs[0].Write(ref input, outType, true);
        }
    }
}