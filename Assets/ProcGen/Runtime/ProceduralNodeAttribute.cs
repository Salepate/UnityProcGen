using System;

namespace ProcGen
{
    /// <summary>
    /// Class attribute used for giving more insights in the editor
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Class)]
    public class ProceduralNodeAttribute : Attribute
    {
        public readonly string[] InputNames;
        public readonly string[] OutputNames;
        /// <summary>
        /// Defines input and output names.
        /// </summary>
        /// <param name="inputCount">number of inputs among the list</param>
        /// <param name="slotNames">inputs and outputs slot names in this order.</param>
        public ProceduralNodeAttribute(int inputCount, params string[] slotNames)
        {
            InputNames = new string[inputCount];
            OutputNames = new string[slotNames.Length - inputCount];
            for (int i = 0; i < inputCount; ++i)
                InputNames[i] = slotNames[i];
            for (int i = inputCount; i < slotNames.Length; ++i)
                OutputNames[i - inputCount] = slotNames[i];
        }
    }
}