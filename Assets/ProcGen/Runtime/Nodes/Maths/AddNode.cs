namespace ProcGen.Nodes.Maths
{
    [ProceduralNode(2, "A","B","Sum")]
    public class AddNode : BaseNode
    {
        public bool Average;
        public override void Initialize()
        {
            Inputs = NodeConnector.CreateInputs(ConnectorType.SourceType, ConnectorType.SourceType);
            Outputs = NodeOutput.CreateOutputs(ConnectorType.SourceType);
        }
        public override void Evaluate()
        {
            ref NodeConnector firstInput = ref Inputs[0];
            ref NodeConnector secondInput = ref Inputs[1];
            ConnectorType connectorType = ConnectorType.SourceType;

            if (firstInput.TryGetAttachedOutput(out NodeOutput inA))
            {
                connectorType = inA.ConnectorType;
            }
            if (secondInput.TryGetAttachedOutput(out NodeOutput inB))
            {
                connectorType = inB.ConnectorType;
            }

            if (connectorType == ConnectorType.SourceType)
                return;
            
            if (connectorType == ConnectorType.Integer)
            {
                Outputs[0].ValueInt = Inputs[0].ReadInteger() + Inputs[1].ReadInteger();
                if (Average)
                    Outputs[0].ValueInt /= 2;
            }
            else // hopefully that will write float, vec2 and vec3
            {
                Outputs[0].ValueVector3 = Inputs[0].ReadVector3() + Inputs[1].ReadVector3();
                if (Average)
                    Outputs[0].ValueVector3 *= 0.5f;
            }
        }
    }
}