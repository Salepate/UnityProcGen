using UnityEngine;

namespace Dirt.ProcGen.Nodes
{
    [System.Serializable]
    public class UnityPerlinNode : BaseNode
    {
        //
        private const int Scale = 0;
        private const int Coordinate = 1;
        private const int Offset = 2;
        // Out
        private const int OutputValue = 0;

        public override void Initialize()
        {
            Inputs = NodeConnector.CreateInputs(
                ConnectorType.Vector2,
                ConnectorType.Vector2,
                ConnectorType.Vector2);

            Outputs = NodeOutput.CreateOutputs(ConnectorType.Float);

        }
        public override void Evaluate()
        {
            Vector2 scale = Inputs[Scale].ReadVector2();
            Vector2 offset = Inputs[Offset].ReadVector2();
            Vector2 coord = Inputs[Coordinate].ReadVector2();
            Outputs[OutputValue].ValueFloat = Mathf.PerlinNoise(coord.x * scale.x + offset.x, coord.y * scale.y + offset.y);
        }
    }
}