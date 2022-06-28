using ProcGen.Connector;
using UnityEngine;

namespace ProcGen.Nodes.Constants
{
    [ProceduralNode(0, "Vector")]
    public class Vector2Node : BaseNode
    {
        public Vector2 Value;

        public override void Initialize()
        {
            Outputs = this.CreateOutputs(ConnectorType.Vector2);
        }
        public override void Evaluate()
        {
            Outputs[0].ValueVector2 = Value;
        }
    }
}