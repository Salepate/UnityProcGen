using ProcGen.Connector;
using UnityEngine;

namespace ProcGen.Nodes
{
    [System.Serializable]
    [ProceduralNode(0, "X","Y")]
    public class TileMapNode : BaseNode
    {
        public Vector2Int Coordinate { get; set; }

        public override void Initialize()
        {
            Outputs = this.CreateOutputs(ConnectorType.Integer, ConnectorType.Integer);
        }
        public override void Evaluate()
        {
            Outputs[0].Value.Int = Coordinate.x;
            Outputs[1].Value.Int = Coordinate.y;
        }
    }
}