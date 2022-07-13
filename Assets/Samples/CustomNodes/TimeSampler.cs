using ProcGen;
using ProcGen.Connector;

namespace ProcGenSamples
{
    [ProceduralNode(0, "Time", "Delta", "Sine Time")]
    public class TimeSampler : BaseNode
    {
        public const int Time = 0;
        public const int Delta = 1;
        public const int SineTime = 2;

        public bool ManualTimeUpdate;

        private float m_Start;
        private float m_SineCache;
        private float m_Elapsed;
        public override void Initialize()
        {
            m_Start = UnityEngine.Time.realtimeSinceStartup;
            Outputs = this.CreateOutputs(ConnectorType.Float, ConnectorType.Float, ConnectorType.Float);
        }
        public override void Evaluate()
        {
            if (!ManualTimeUpdate)
                CacheTimeValue();
            Outputs[Time].Value.Float = m_Elapsed;
            Outputs[Delta].Value.Float = UnityEngine.Time.deltaTime;
            Outputs[SineTime].Value.Float = m_SineCache;
        }

        public void CacheTimeValue()
        {
            m_Elapsed = UnityEngine.Time.realtimeSinceStartup - m_Start;
            m_SineCache = UnityEngine.Mathf.Sin(m_Elapsed * UnityEngine.Mathf.PI * 2f);
        }
    }
}