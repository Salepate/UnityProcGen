using ProcGen.Buffer;
using ProcGen.Connector;
using System.Collections.Generic;

namespace ProcGen.Nodes.Output
{
    [ProceduralNode(5, "In (1)", "In (2)", "In (3)", "In (4)", "In (5)")]
    public class GraphOutputNode : BaseNode, IMasterNode
    {
        public List<ConnectorType> BufferContent;

        private GraphBuffer m_Buffer;

        public GraphOutputNode()
        {
            BufferContent = new List<ConnectorType>();
            Inputs = this.CreateInputs(0);
        }

        public override void Initialize()
        {
            Inputs = this.CreateInputs(BufferContent.ToArray());
        }

        public override void Evaluate()
        {
            for(int i = 0; i < Inputs.Length; ++i)
            {
                m_Buffer.WriteBytes(ref Inputs[i], i);
            }
        }

        // IMasterNode
        public MasterNodeSettings GetSettings()
        {
            return new MasterNodeSettings()
            {
                HasBuffer = true,
                BufferLayout = BufferContent
            };
        }

        public void SetBuffer(GraphBuffer buffer)
        {
            m_Buffer = buffer;
        }
    }
}