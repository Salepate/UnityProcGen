using ProcGen;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProcGenEditor
{
    using PortTuple = System.Tuple<bool, int>;
    public class ProcGenGraphNodeView : Node
    {
        private static readonly string[] s_DefaultConnectorNames = new string[1] { "Input" };
        private static readonly string[] s_DefaultOutputNames = new string[1] { "Output" };
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

            string nodeName = nodeData.GetType().Name;
            if (nodeName.EndsWith("Node"))
                nodeName = nodeName.Substring(0, nodeName.Length - 4);
            
            this.Q<Label>("title-label").text = ObjectNames.NicifyVariableName(nodeName);

            // TODO: store in cache if performance is an issue
            string[] outputNames = s_DefaultOutputNames;
            ProceduralNodeAttribute procNode = nodeData.GetType().GetCustomAttribute<ProceduralNodeAttribute>();
            string[] connectorNames = procNode != null ? procNode.ConnectorNames : s_DefaultConnectorNames;
            if (procNode != null && procNode.OutputNames != null)
                outputNames = procNode.OutputNames;

            for(int i = 0; Node.Inputs != null && i < Node.Inputs.Length; ++i)
            {
                int connectorNameIndex = Mathf.Min(i, connectorNames.Length - 1);
                string connectorName = connectorNames[connectorNameIndex];
                System.Type portType = ConnectorHelper.GetAssociatedType(Node.Inputs[i].ConnectorType);
                var port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, portType);
                port.portName = connectorName;
                port.userData = Node.Inputs[i].ConnectorType;
                inputContainer.Add(port);
                m_PortMap.Add(port, new PortTuple(false, i));
                m_InversePortMap.Add(new PortTuple(false, i), port);
            }
            for(int i = 0; i < Node.Outputs.Length; ++i)
            {
                int outputNameIndex = Mathf.Min(i, outputNames.Length - 1);
                string outputName = outputNames[outputNameIndex];
                System.Type portType = ConnectorHelper.GetAssociatedType(Node.Outputs[i].ConnectorType);
                var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, portType);
                port.portName = outputName;
                port.userData = Node.Outputs[i].ConnectorType;
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