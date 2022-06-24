namespace Dirt.ProcGen
{
    public class ProceduralNodeAttribute : System.Attribute
    {
        public string[] ConnectorNames;
        public string[] OutputNames;
        /// <summary>
        /// Defines input names;
        /// </summary>
        /// <param name="inputCount"></param>
        /// <param name="slotNames"></param>
        public ProceduralNodeAttribute(params string[] connectorNames)
        {
            ConnectorNames = connectorNames;
        }

        /// <summary>
        /// Defines input and output names.
        /// </summary>
        /// <param name="inputCount"></param>
        /// <param name="slotNames"></param>
        public ProceduralNodeAttribute(int inputCount, params string[] slotNames)
        {
            ConnectorNames = new string[inputCount];
            OutputNames = new string[slotNames.Length - inputCount];
            for (int i = 0; i < inputCount; ++i)
                ConnectorNames[i] = slotNames[i];
            for (int i = inputCount; i < slotNames.Length; ++i)
                OutputNames[i - inputCount] = slotNames[i];
        }
    }
}
