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
            Outputs = NodeOutput.CreateOutputs(ConnectorType.Integer, ConnectorType.Integer);
        }
        public override void Evaluate()
        {
            Outputs[0].ValueInt = Coordinate.x;
            Outputs[1].ValueInt = Coordinate.y;
        }
    }
}