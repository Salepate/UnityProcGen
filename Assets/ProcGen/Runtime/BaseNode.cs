namespace Dirt.ProcGen
{
    public abstract class BaseNode
    {
        /// <summary>
        /// Overrides to set node inputs
        /// </summary>
        public NodeConnector[] Inputs { get; protected set; }
        public NodeOutput[] Outputs { get; protected set; }

        /// <summary>
        /// Override to initialize node, inputs and outputs
        /// </summary>
        public virtual void Initialize() { }
        public abstract void Evaluate();

        public BaseNode()
        {
            Inputs = new NodeConnector[0];
            Outputs = new NodeOutput[0];
        }
    }
}