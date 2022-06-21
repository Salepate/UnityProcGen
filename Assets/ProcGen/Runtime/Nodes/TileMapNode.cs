using UnityEngine;

namespace Dirt.ProcGen
{
    [System.Serializable]
    public class TileMapNode : BaseNode
    {
        public Vector2Int m_Coordinate;

        public override void Initialize()
        {
            Outputs = NodeOutput.CreateOutputs(ConnectorType.Integer, ConnectorType.Integer);
        }
        public override void Evaluate()
        {
            Outputs[0].ValueInt = m_Coordinate.x;
            Outputs[1].ValueInt = m_Coordinate.y;
        }
    }
}