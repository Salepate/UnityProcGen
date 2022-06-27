using ProcGen;

namespace ProcGenSamples
{
    [ProceduralNode(1, "Height")]
    public class HeightMapNode : BaseNode
    {
        public float Height { get; private set; }

        public override void Initialize()
        {
            Inputs = NodeConnector.CreateInputs(ConnectorType.Float);
        }
        public override void Evaluate()
        {
            Height = Inputs[0].ReadFloat();
        }
    }
}