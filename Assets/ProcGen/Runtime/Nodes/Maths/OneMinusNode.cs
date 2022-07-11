using ProcGen.Connector;
using UnityEngine;

namespace ProcGen.Nodes.Maths
{
    [ProceduralNode(1, "Source", "One - Source")]
    public class OneMinusNode : BaseNode
    {
        public override void Initialize()
        {
            Inputs = this.CreateInputs(ConnectorType.SourceType);
            Outputs = this.CreateOutputs(ConnectorType.SourceType);
        }
        public override void Evaluate()
        {
            if ( Inputs[0].TryGetAttachedOutput(out NodeOutput source))
            {
                Outputs[0].ConnectorType = source.ConnectorType;

                if ( source.ConnectorType == ConnectorType.Integer )
                {
                    Outputs[0].Value.Int = 1 - Inputs[0].ReadInteger();
                }
                else
                {
                    Outputs[0].Value.Vec3 = Vector3.one - Inputs[0].ReadVector3();
                }
            }
        }
    }
}