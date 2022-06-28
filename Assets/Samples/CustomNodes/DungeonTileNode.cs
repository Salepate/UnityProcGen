using ProcGen;
using ProcGen.Connector;

namespace ProcGenSamples
{
    public class DungeonTileNode : BaseNode
    {
        public float Threshold;
        public bool IsTile { get; private set; }
        public override void Initialize()
        {
            Inputs = this.CreateInputs(ConnectorType.Float);
        }
        public override void Evaluate()
        {
            IsTile = Inputs[0].ReadFloat() >= Threshold;
        }
    }
}