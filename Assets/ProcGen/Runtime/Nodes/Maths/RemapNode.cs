using ProcGen.Connector;
using UnityEngine;

namespace ProcGen.Nodes.Maths
{
    [ProceduralNode(5, "Input", "PrevMin", "PrevMax", "NewMin", "NewMax", "Output")]
    public class RemapNode : BaseNode
    {
        public const int Input = 0;
        public const int PrevMin = 1;
        public const int PrevMax = 2;
        public const int NewMin  = 3;
        public const int NewMax  = 4;
        public override void Initialize()
        {
            Inputs = this.CreateInputs(ConnectorType.SourceType, ConnectorType.Float, ConnectorType.Float, ConnectorType.Float, ConnectorType.Float);
            Inputs[PrevMin].Initial.Float = 0f;
            Inputs[PrevMax].Initial.Float = 1f;
            Inputs[NewMin].Initial.Float = 0f;
            Inputs[NewMax].Initial.Float = 1f;
            Outputs = this.CreateOutputs(ConnectorType.SourceType);
        }
        public override void Evaluate()
        {
            ref NodeInput inputValue = ref Inputs[Input];
            ref NodeOutput outputValue = ref Outputs[0];

            if (inputValue.TryGetAttachedOutput(out NodeOutput source))
            {
                outputValue.ConnectorType = source.ConnectorType;
                switch (source.ConnectorType)
                {
                    case ConnectorType.Integer:
                        outputValue.Value.Int = RemapValue(inputValue.ReadInteger(), Get(PrevMin).ReadInteger(), Get(PrevMax).ReadInteger(), Get(NewMin).ReadInteger(), Get(NewMax).ReadInteger());
                        break;
                    case ConnectorType.Float:
                        outputValue.Value.Float = RemapValue(inputValue.ReadFloat(), Get(PrevMin).ReadFloat(), Get(PrevMax).ReadFloat(), Get(NewMin).ReadFloat(), Get(NewMax).ReadFloat());
                        break;
                    case ConnectorType.Vector2:
                    case ConnectorType.Vector3:
                        Vector3 inputVec = inputValue.ReadVector3();
                        float prevMin = Get(PrevMin).ReadFloat();
                        float prevMax = Get(PrevMax).ReadFloat();
                        float newMin = Get(NewMin).ReadFloat();
                        float newMax = Get(NewMax).ReadFloat();
                        outputValue.Value.Vec3 = new Vector3()
                        {
                            x = RemapValue(inputVec.x, prevMin, prevMax, newMin, newMax),
                            y = RemapValue(inputVec.y, prevMin, prevMax, newMin, newMax),
                            z = RemapValue(inputVec.z, prevMin, prevMax, newMin, newMax)
                        };
                        break;
                }
            }
        }

        private ref NodeInput Get(int index)
        {
            return ref Inputs[index];
        }

        private float RemapValue(float value, float prevMin, float prevMax, float newMin, float newMax)
        {
            float ratio = (value - prevMin) / (prevMax - prevMin);
            return newMin + ratio * (newMax - newMin);
        }

        private int RemapValue(int value, int prevMin, int prevMax, int newMin, int newMax)
        {
            float ratio = (float)(value - prevMin) / (prevMax - prevMin);
            return (int)(newMin + ratio * (newMax - newMin));
        }
    }
}