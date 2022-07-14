using ProcGen;
using ProcGen.Connector;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ProcGenEditor.GraphElems
{
    public class NodeInputView : VisualElement
    {
        private BaseNode m_Node;
        private int m_Slot;

        private VisualElement Container;
        private System.Action m_UpdateAction;
        public NodeInputView(BaseNode node, int inputSlot, System.Action updateAction)
        {
            m_Node = node;
            m_Slot = inputSlot;
            m_UpdateAction = updateAction;
            AddToClassList("input-view");
            Container = new VisualElement() { name = "Container" };
            Add(Container);
            CreateControls();
            UpdateControlVisibility();
        }

        public void CreateControls()
        {
            bool visible = m_Slot < m_Node.Inputs.Length;


            if (visible)
            {
                ref NodeInput input = ref m_Node.Inputs[m_Slot];
                if ( input.ConnectorType != ConnectorType.SourceType )
                {
                    Container.Add(SpawnControlElement(ref input));

                }
            }
        }

        public VisualElement SpawnControlElement(ref NodeInput input)
        {
            switch (input.ConnectorType)
            {
                case ConnectorType.Integer:
                    IntegerField integerFld = new IntegerField() { value = input.Initial.Int };
                    integerFld.RegisterValueChangedCallback((valueEvt) => { m_Node.Inputs[m_Slot].Initial.Int = valueEvt.newValue; m_UpdateAction.Invoke(); });
                    return integerFld;
                case ConnectorType.Float:
                    FloatField floatFld = new FloatField() { value = input.Initial.Float };
                    floatFld.RegisterValueChangedCallback((valueEvt) => { m_Node.Inputs[m_Slot].Initial.Float = valueEvt.newValue; m_UpdateAction.Invoke(); });
                    return floatFld;
                case ConnectorType.Vector2:
                    Vector2Field vec2Fld = new Vector2Field() { value = input.Initial.Vec2 };
                    vec2Fld.RegisterValueChangedCallback((valueEvt) => { m_Node.Inputs[m_Slot].Initial.Vec2 = valueEvt.newValue; m_UpdateAction.Invoke(); });
                    return vec2Fld;
                case ConnectorType.Vector3:
                    Vector3Field vec3Fld = new Vector3Field() { value = input.Initial.Vec3 };
                    vec3Fld.RegisterValueChangedCallback((valueEvt) => { m_Node.Inputs[m_Slot].Initial.Vec3 = valueEvt.newValue; m_UpdateAction.Invoke(); });
                    return vec3Fld;
            }
            return null;
        }

        public void UpdateControlVisibility()
        {
            bool visible = m_Slot < m_Node.Inputs.Length && !m_Node.Inputs[m_Slot].IsConnectorValid();
            Container.visible = visible;
        }
    }
}