using ProcGen.Buffer;
using ProcGen.Connector;
using System.Collections.Generic;

namespace ProcGen
{
    public interface IMasterNode
    {
        public MasterNodeSettings GetSettings();
        void SetBuffer(GraphBuffer buffer);
    }

    public struct MasterNodeSettings
    {
        public bool HasBuffer;
        public List<ConnectorType> BufferLayout;
    }

}