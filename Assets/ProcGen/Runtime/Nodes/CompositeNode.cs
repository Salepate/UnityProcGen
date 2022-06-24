using UnityEngine;

namespace Dirt.ProcGen
{
    [ProceduralNode(2, "X","Y","Vector")]
    public class CreateVector2Node : BaseNode
    {
        public const int X = 0;
        public const int Y = 1;
        public override void Initialize()
        {
            Inputs = NodeConnector.CreateInputs(ConnectorType.Float, ConnectorType.Float);
            Outputs = NodeOutput.CreateOutputs(ConnectorType.Vector2);
        }
        public override void Evaluate()
        {
            Vector2 saucisse = new Vector2(Inputs[X].ReadFloat(), Inputs[Y].ReadFloat());
            Outputs[0].ValueVector2 = new Vector2(Inputs[X].ReadFloat(), Inputs[Y].ReadFloat());
        }
    }

}