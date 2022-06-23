using Dirt.ProcGen;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProcGenEditor
{
    using PortTuple = System.Tuple<bool, int>;
    public class ProcGenGraphNodeView : Node
    {
        public Vector2 Origin { get; private set; }
        public BaseNode Node { get; private set; }

        private Dictionary<Port, PortTuple> m_PortMap;
        private Dictionary<PortTuple, Port> m_InversePortMap;


        public bool TryGetPortData(Port port, out int slotIndex, out bool isOutput)
        {
            slotIndex = 0;
            isOutput = false;

            if ( m_PortMap.TryGetValue(port, out PortTuple portData))
            {
                slotIndex = portData.Item2;
                isOutput = portData.Item1;
                return true;
            }
            return false;
        }

        public bool TryGetPort(int slotIndex, bool isOutput, out Port port)
        {
            return m_InversePortMap.TryGetValue(new PortTuple(isOutput, slotIndex), out port);
        }


        public ProcGenGraphNodeView()
        {
            m_PortMap = new Dictionary<Port, PortTuple>();
            m_InversePortMap = new Dictionary<PortTuple, Port>();
        }

        public ProcGenGraphNodeView(BaseNode nodeData) : this()
        {
            Node = nodeData;

            this.Q<Label>("title-label").text = nodeData.GetType().Name;

            for(int i = 0; Node.Inputs != null && i < Node.Inputs.Length; ++i)
            {
                System.Type portType = ConnectorHelper.GetAssociatedType(Node.Inputs[i].ConnectorType);
                var port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, portType);
                //port.contentContainer.Add(new SliderInt());
                inputContainer.Add(port);
                m_PortMap.Add(port, new PortTuple(false, i));
                m_InversePortMap.Add(new PortTuple(false, i), port);
            }
            for(int i = 0; i < Node.Outputs.Length; ++i)
            {
                System.Type portType = ConnectorHelper.GetAssociatedType(Node.Outputs[i].ConnectorType);
                var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, portType);
                outputContainer.Add(port);
                m_PortMap.Add(port, new PortTuple(true, i));
                m_InversePortMap.Add(new PortTuple(true, i), port);
            }

            var customGUI = new IMGUIContainer();
            customGUI.userData = Editor.CreateEditor(nodeData, typeof(BaseNodeInspector));
            customGUI.contextType = ContextType.Editor;
            customGUI.onGUIHandler = () => ((Editor)customGUI.userData).OnInspectorGUI();
            extensionContainer.Add(customGUI);
            RefreshExpandedState();
        }

        internal void SetOrigin(Vector2 newOrigin)
        {
            Origin = newOrigin;

            SetPosition(new Rect(Origin, new Vector2(180f, 80f)));
        }

        protected override void ToggleCollapse()
        {
            expanded = !expanded;
        }

        public override bool expanded
        {
            get => base.expanded;
            set
            {
                if (base.expanded == value)
                    return;

                base.expanded = value;

                RefreshExpandedState(); // Necessary b/c we can't override enough Node.cs functions to update only what's needed
            }
        }
    }
}