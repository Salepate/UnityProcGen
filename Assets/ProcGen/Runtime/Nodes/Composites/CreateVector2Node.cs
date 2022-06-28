using ProcGen.Connector;
using UnityEngine;

namespace ProcGen.Nodes.Composites
{
    [ProceduralNode(2, "X", "Y", "Vector")]
    public class CreateVector2Node : BaseNode
    {
        public const int X = 0;
        public const int Y = 1;

        public override void Initialize()
        {
            Inputs = this.CreateInputs(ConnectorType.Float, ConnectorType.Float);
            Outputs = this.CreateOutputs(ConnectorType.Vector2);
        }

        public override void Evaluate()
        {
            Outputs[0].ValueVector2 = new Vector2(Inputs[X].ReadFloat(), Inputs[Y].ReadFloat());
        }
    }
}