using UnityEngine;

namespace ProcGen.Nodes.Composites
{
    [ProceduralNode(3, "X", "Y", "Z", "Vector")]
    public class CreateVector3Node : BaseNode
    {
        public const int X = 0;
        public const int Y = 1;
        public const int Z = 2;

        public override void Initialize()
        {
            Inputs = NodeConnector.CreateInputs(ConnectorType.Float, ConnectorType.Float, ConnectorType.Float);
            Outputs = NodeOutput.CreateOutputs(ConnectorType.Vector3);
        }

        public override void Evaluate()
        {
            Outputs[0].ValueVector3 = new Vector3(Inputs[X].ReadFloat(), Inputs[Y].ReadFloat(), Inputs[Z].ReadFloat());
        }
    }
}