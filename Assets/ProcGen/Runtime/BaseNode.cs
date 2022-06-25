namespace ProcGen
{
#if UNITY_EDITOR
    public abstract class BaseNode : UnityEngine.ScriptableObject // only used for inspecting fields
#else
    public abstract class BaseNode
#endif
    {
        public int NodeIndex { get; private set; }
        /// <summary>
        /// Overrides to declare node inputs
        /// </summary>
        public NodeConnector[] Inputs { get; protected set; }
        /// <summary>
        /// Overrides to declare node outputs
        /// </summary>
        public NodeOutput[] Outputs { get; protected set; }

        public BaseNode()
        {
            NodeIndex = -1;
            Inputs = new NodeConnector[0];
            Outputs = new NodeOutput[0];
        }

        public void SetIndex(int index)
        {
            NodeIndex = index;
        }

        /// <summary>
        /// Invoked at runtime
        /// </summary>
        public abstract void Evaluate();
        /// <summary>
        /// Override to initialize node, inputs and outputs
        /// </summary>
        public virtual void Initialize() { }
    }
}