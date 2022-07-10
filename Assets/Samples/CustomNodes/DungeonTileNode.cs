using ProcGen;
using ProcGen.Connector;
using UnityEngine;

namespace ProcGenSamples
{
    [ProceduralNode(2, "Noise", "Color")]
    public class DungeonTileNode : BaseNode
    {
        public float Threshold;
        public bool IsTile { get; private set; }
        public Color TileColor { get; private set; }
        public override void Initialize()
        {
            Inputs = this.CreateInputs(ConnectorType.Float, ConnectorType.Vector3);
        }
        public override void Evaluate()
        {
            IsTile = Inputs[0].ReadFloat() >= Threshold;
            if ( IsTile )
            {
                Vector3 vCol = Inputs[1].ReadVector3();
                TileColor = new Color(vCol.x, vCol.y, vCol.z);
            }
        }
    }
}