using ProcGen;
using UnityEngine;

namespace ProcGen.Debug
{
    public class GraphDebuggerBehaviour : MonoBehaviour
    {
        public GenerativeGraphInstance Instance { get; private set; }
        private bool m_GraphHasChanged;
        public void SetInstance(GenerativeGraphInstance graphInst)
        {
            Instance = graphInst;
        }

        public void NotifyGraphChanged()
        {
            m_GraphHasChanged = true;
        }

        private void Update()
        {
            if ( m_GraphHasChanged )
            {
                Instance.Runtime.ComputeExecutionTree();
                Instance.OnGraphUpdate?.Invoke();
                m_GraphHasChanged = false;
            }
        }
    }
}