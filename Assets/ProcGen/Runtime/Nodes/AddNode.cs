namespace ProcGen.Nodes
{
    [ProceduralNode(2, "A","B","Sum")]
    public class AddNode : BaseNode
    {
        public ConnectorType Connector;
        public bool Average;
        public override void Initialize()
        {
            Inputs = NodeConnector.CreateInputs(Connector, Connector);
            Outputs = NodeOutput.CreateOutputs(Connector);
        }
        public override void Evaluate()
        {
            // TODO: do something
            if (Connector == ConnectorType.Integer)
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