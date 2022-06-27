using UnityEngine;

namespace ProcGen.Nodes.Composites
{
    [ProceduralNode(1, "Vector", "X", "Y", "Z")]
    public class SplitVectorNode : BaseNode
    {
        public override void Initialize()
        {
            Inputs = NodeConnector.CreateInputs(ConnectorType.Vector3);
            Outputs = NodeOutput.CreateOutputs(ConnectorType.Float, ConnectorType.Float, ConnectorType.Float);
        }
        public override void Evaluate()
        {
            Vector3 input = Inputs[0].ReadVector3();
            Outputs[0].ValueFloat = input.x;
            Outputs[1].ValueFloat = input.y;
            Outputs[2].ValueFloat = input.z;
        }
    }
}