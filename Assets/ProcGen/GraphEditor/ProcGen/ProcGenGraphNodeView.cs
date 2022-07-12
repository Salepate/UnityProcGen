using ProcGen;
using ProcGen.Connector;
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

        public System.Action DataUpdate;
        private static readonly string[] s_DefaultConnectorNames = new string[1] { "Input" };
        private static readonly string[] s_DefaultOutputNames = new string[1] { "Output" };
        public Vector2 Origin { get; private set; }
        public BaseNode Node { get; private set; }
        public Group ParentGroup { get; set; }

        private Dictionary<Port, PortTuple> m_PortMap;
        private Dictionary<PortTuple, Port> m_InversePortMap;

        public IMGUIContainer IMGUI { get; private set; }
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

            this.Q<Label>("title-label").text = ProcGenEditorHelper.FormatNodeName(nodeData.GetType().Name);

            InstantiatePorts(true);
            InstantiatePorts(false);
            BaseNodeInspector nodeEditor = (BaseNodeInspector) Editor.CreateEditor(nodeData);
            nodeEditor.OnPortUpdate += OnPortUpdate;
            IMGUI = new IMGUIContainer();
            IMGUI.userData = nodeEditor;
            IMGUI.contextType = ContextType.Editor;
            IMGUI.onGUIHandler = () =>
            {
                EditorGUI.BeginChangeCheck();
                ((Editor)IMGUI.userData).OnInspectorGUI();
                if ( EditorGUI.EndChangeCheck())
                {
                    DataUpdate?.Invoke();
                }
            };
            extensionContainer.Add(IMGUI);
            RefreshExpandedState();
        }

        public void OnPortUpdate()
        {
            Node.Initialize();
            //InstantiatePorts(true);
            InstantiatePorts(false);
        }

        public void InstantiatePorts(bool outputPorts)
        {
            VisualElement container;
            System.Array arr;
            ProceduralNodeAttribute procNode = Node.GetType().GetCustomAttribute<ProceduralNodeAttribute>();
            string[] connectorNames;
            Direction portDir = Direction.Input;
            Port.Capacity portCapacity = Port.Capacity.Single;

            if (outputPorts)
            {
                connectorNames = procNode != null ? procNode.OutputNames : s_DefaultOutputNames;
                container = outputContainer;
                arr = Node.Outputs;
                portCapacity = Port.Capacity.Multi;
                portDir = Direction.Output;
            }
            else
            {
                connectorNames = procNode != null ? procNode.InputNames : s_DefaultConnectorNames;
                container = inputContainer;
                arr = Node.Inputs;

            }

            Dictionary<int, Edge> portConnections = new Dictionary<int, Edge>();

            foreach (KeyValuePair<Port, PortTuple> portEntry in m_PortMap)
            {
                Port p = portEntry.Key;
                if (p.connected && portEntry.Value.Item1 == outputPorts)
                {
                    foreach (var conn in p.connections)
                    {
                        portConnections.Add(portEntry.Value.Item2, conn);
                    }
                }
            }

            List<Port> ports = new List<Port>();
            foreach(var port in m_PortMap)
            {
                if (port.Value.Item1 == outputPorts)
                {
                    container.Remove(port.Key);
                    m_InversePortMap.Remove(port.Value);
                    ports.Add(port.Key);
                }
            }
            ports.ForEach(p =>
            {
                m_PortMap.Remove(p);
            });


            for (int i = 0; arr != null && i < arr.Length; ++i)
            {
                int connectorNameIndex = Mathf.Min(i, connectorNames.Length - 1);
                string connectorName = connectorNames[connectorNameIndex];
                System.Type portType;
                ConnectorType connectorType;

                if ( outputPorts )
                    connectorType = Node.Outputs[i].ConnectorType;
                else
                    connectorType = Node.Inputs[i].ConnectorType;

                portType = ConnectorHelper.GetAssociatedType(connectorType);

                var port = InstantiatePort(Orientation.Horizontal, portDir, portCapacity, portType);
                port.portName = connectorName;
                port.userData = connectorType;
                container.Add(port);
                m_PortMap.Add(port, new PortTuple(outputPorts, i));
                m_InversePortMap.Add(new PortTuple(outputPorts, i), port);

                if (portConnections.TryGetValue(i, out Edge existingEdge))
                {
                    bool canConvert = false;
                    Port inputPort = outputPorts ? existingEdge.input : port;
                    Port outputPort = outputPorts ? port : existingEdge.output;
                    canConvert = ConnectorHelper.CanConvert((ConnectorType)inputPort.userData, (ConnectorType)outputPort.userData);

                    if ( canConvert )
                    {
                        existingEdge.input = inputPort;
                        port.Connect(existingEdge);
                        ProcGenGraphNodeView inputNodeView = (ProcGenGraphNodeView)inputPort.node;
                        ProcGenGraphNodeView outputNodeView = (ProcGenGraphNodeView)outputPort.node;
                        int inputIndex = inputNodeView.m_PortMap[inputPort].Item2;
                        int outputIndex = outputNodeView.m_PortMap[outputPort].Item2;
                        ref NodeInput nodeInput = ref inputNodeView.Node.Inputs[inputIndex];
                        nodeInput.Connect(outputNodeView.Node, outputIndex);
                        portConnections.Remove(i);
                    }
                }
            }
            foreach (var leftOver in portConnections)
            {
                Edge edge = leftOver.Value;
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                edge.parent.Remove(edge);
            }
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