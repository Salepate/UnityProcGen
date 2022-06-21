namespace Dirt.ProcGen
{
    public abstract class BaseNode
    {
        /// <summary>
        /// Overrides to set node inputs
        /// </summary>
        public virtual NodeConnector[] Inputs { get; protected set; }
        public virtual NodeOutput[] Outputs { get; protected set; }

        /// <summary>
        /// Override to initialize node, inputs and outputs
        /// </summary>
        public virtual void Initialize() { }
        public abstract void Evaluate();
    }
}