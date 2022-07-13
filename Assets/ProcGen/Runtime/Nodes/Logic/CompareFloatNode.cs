using ProcGen.Connector;
using ProcGen.Utils;

namespace ProcGen.Nodes.Logic
{

    [ProceduralNode(strict: true, 2, "A", "B", "Condition")]
    public class CompareFloatNode : BaseNode
    {
        [ComparisonEnum]
        public ComparisonType Comparison;
        // in
        public const int A = 0;
        public const int B = 1;

        public override void Initialize()
        {
            Inputs = this.CreateInputs(ConnectorType.Float, ConnectorType.Float);
            Outputs = this.CreateOutputs(ConnectorType.Integer);
        }
        public override void Evaluate()
        {
            float valueA = Inputs[A].ReadFloat();
            float valueB = Inputs[B].ReadFloat();
            switch (Comparison)
            {
                case ComparisonType.Equal: AssignOutput(valueA == valueB); break;
                case ComparisonType.NotEqual: AssignOutput(valueA != valueB); break;
                case ComparisonType.GreaterThan: AssignOutput(valueA > valueB); break;
                case ComparisonType.GreaterOrEqual: AssignOutput(valueA >= valueB); break;
                case ComparisonType.LessThan: AssignOutput(valueA < valueB); break;
                case ComparisonType.LessOrEqual: AssignOutput(valueA <= valueB); break;
            }
        }

        public void AssignOutput(bool fillCondition)
        {
            Outputs[0].Value.Int = fillCondition ? 1 : 0;
        }
    }
}