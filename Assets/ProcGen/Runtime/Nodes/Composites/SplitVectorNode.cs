using ProcGen.Connector;
using UnityEngine;

namespace ProcGen.Nodes.Composites
{
    [ProceduralNode(1, "Vector", "X", "Y", "Z")]
    public class SplitVectorNode : BaseNode
    {
        public override void Initialize()
        {
            Inputs = this.CreateInputs(ConnectorType.Vector3);
            Outputs = this.CreateOutputs(ConnectorType.Float, ConnectorType.Float, ConnectorType.Float);
        }
        public override void Evaluate()
        {
            Vector3 input = Inputs[0].ReadVector3();
            Outputs[0].Value.Float = input.x;
            Outputs[1].Value.Float = input.y;
            Outputs[2].Value.Float = input.z;
        }
    }
}