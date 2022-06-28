using ProcGen.Connector;
using UnityEngine;

namespace ProcGen.Nodes
{
    [System.Serializable]
    [ProceduralNode(3, "Coordinates", "Scale", "Offset", "Noise")]
    public class UnityPerlinNode : BaseNode
    {
        // inputs
        private const int Coordinate = 0;
        private const int Scale = 1;
        private const int Offset = 2;
        // output
        private const int OutputValue = 0;

        public override void Initialize()
        {
            Inputs = this.CreateInputs(
                ConnectorType.Vector2,
                ConnectorType.Vector2,
                ConnectorType.Vector2);

            Outputs = this.CreateOutputs(ConnectorType.Float);

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