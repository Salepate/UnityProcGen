using ProcGen;
using ProcGen.Connector;

namespace ProcGenSamples
{
    [ProceduralNode(1, "Height")]
    public class HeightMapNode : BaseNode
    {
        public float Height { get; private set; }

        public override void Initialize()
        {
            Inputs = this.CreateInputs(ConnectorType.Float);
        }
        public override void Evaluate()
        {
            Height = Inputs[0].ReadFloat();
        }
    }
}