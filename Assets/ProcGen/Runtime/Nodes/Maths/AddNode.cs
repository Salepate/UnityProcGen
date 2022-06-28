using ProcGen.Connector;

namespace ProcGen.Nodes.Maths
{
    [ProceduralNode(2, "A","B","Sum")]
    public class AddNode : BaseNode
    {
        public bool Average;
        public override void Initialize()
        {
            Inputs = this.CreateInputs(ConnectorType.SourceType, ConnectorType.SourceType);
            Outputs = this.CreateOutputs(ConnectorType.SourceType);
        }
        public override void Evaluate()
        {
            ref NodeInput firstInput = ref Inputs[0];
            ref NodeInput secondInput = ref Inputs[1];
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

            Outputs[0].ConnectorType = connectorType;
            
            if (connectorType == ConnectorType.Integer)
            {
                int factor = Average ? 2 : 1;
                Outputs[0].ValueInt = (Inputs[0].ReadInteger() + Inputs[1].ReadInteger()) / factor;
            }
            else
            {
                float ratio = Average ? 0.5f : 1f;
                Outputs[0].ValueVector3 =(Inputs[0].ReadVector3() + Inputs[1].ReadVector3()) * ratio;
            }
        }
    }
}