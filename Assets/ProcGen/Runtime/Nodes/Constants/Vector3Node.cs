using ProcGen.Connector;
using UnityEngine;

namespace ProcGen.Nodes.Constants
{
    [ProceduralNode(0, "Vector")]
    public class Vector3Node : BaseNode
    {
        public Vector3 Value;

        public override void Initialize()
        {
            Outputs = this.CreateOutputs(ConnectorType.Vector3);
        }
        public override void Evaluate()
        {
            Outputs[0].Value.Vec3 = Value;
        }
    }
}