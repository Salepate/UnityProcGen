using ProcGen;
using ProcGen.Connector;
using ProcGen.Debug;
using ProcGen.Serialization;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProcGenEditor
{
    public class ProcGenGraphEditor : EditorWindow
    {
        public GenerativeGraphInstance GraphInstance { get; private set; }
        public ProcGenGraphElementInspector ElementInspector { get; private set; }
        private SearchWindowProvider m_Provider;
        private GraphDebuggerBehaviour m_GraphDebugger;


        [MenuItem("Window/General/Generative Graph Editor")]
        private static void OpenEditor()
        {
            ShowMenu();
            GetWindow<ProcGenGraphEditor>().LoadGenerativeGraph(new GenerativeGraphInstance() { Graph = ScriptableObject.CreateInstance<GenerativeGraph>() });
        }

        public static void QuickLoadGraph(GenerativeGraph graph)
        {
            ShowMenu();
            GetWindow<ProcGenGraphEditor>().LoadGenerativeGraph(new GenerativeGraphInstance() { Graph = graph });
        }

        public static void DebugRuntime(GenerativeGraphInstance graphInstance)
        {
            ShowMenu();
            var editor = GetWindow<ProcGenGraphEditor>();
            editor.LoadGenerativeGraph(graphInstance);
            GraphDebuggerBehaviour dbg = GameObject.FindObjectOfType<GraphDebuggerBehaviour>();
            if (dbg == null)
            {
                GameObject dbgObj = new GameObject("GraphDebugger");
                dbgObj.hideFlags = HideFlags.DontSave | HideFlags.DontSaveInEditor;
                dbg = dbgObj.AddComponent<GraphDebuggerBehaviour>();
            }
            editor.AttachDebugger(dbg);
        }

        public void AttachDebugger(GraphDebuggerBehaviour dbg)
        {
            m_GraphDebugger = dbg;
            dbg.SetInstance(GraphInstance);
        }

        private static void ShowMenu()
        {
            ProcGenGraphEditor win = GetWindow<ProcGenGraphEditor>("Generative Graph");
            win.position = new Rect(win.position.position, new Vector2(800f, 600f));
            win.Show(true);
        }

        private void OnEnable()
        {
            var vt = Resources.Load<VisualTreeAsset>("window-procgen");
            var template = vt.CloneTree();
            rootVisualElement.Add(template);
            template.StretchToParentSize();
            m_Provider = ScriptableObject.CreateInstance<SearchWindowProvider>();
            m_Provider.Editor = this;
            VisualElement toolbar = rootVisualElement.Q<VisualElement>("editor-toolbar");
            toolbar.Q<Button>("button-save").clicked += SaveGenerativeGraph;
            toolbar.Q<Button>("button-open").clicked += OpenGenerativeGraph;
            toolbar.Q<Button>("button-inspector").clicked += ToggleInspector;
            ElementInspector = new ProcGenGraphElementInspector(rootVisualElement.Q("graphInspector"));
            ToggleInspector();
        }

        public void ResetGraph()
        {
            rootVisualElement.Q("graphRoot").Clear();
            var style = Resources.Load<StyleSheet>("procgen-graph-style");
            ProcGenGraphView graphView = new ProcGenGraphView();
            rootVisualElement.Q("graphRoot").Add(graphView);
            graphView.StretchToParentSize();
            graphView.AddManipulator(new ContentDragger());
            graphView.AddManipulator(new SelectionDragger());
            graphView.AddManipulator(new ClickSelector());
            graphView.AddManipulator(new RectangleSelector());
            graphView.SetupZoom(0.5f, ContentZoomer.DefaultMaxScale);
            graphView.NotifyGraphUpdate = NotifyGraphChange;
            graphView.styleSheets.Add(style);
            graphView.NotifyElementInspect += ElementInspector.InspectElement;
            rootVisualElement.Q("graphRoot").RegisterCallback<KeyDownEvent>(OnKey);

            m_Provider.GraphView = graphView;
        }

        private void LoadGenerativeGraph(GenerativeGraphInstance graphInstance)
        {
            ResetGraph();
            GraphInstance = graphInstance;

            if (GraphInstance.Runtime == null)
                GraphInstance.Runtime = graphInstance.Graph.Deserialize(ProcGenSerialization.SerializationSettings, ProcGenSerialization.NodeConverter);

            GenerativeGraph graph = graphInstance.Graph;
            m_Provider.GraphView.GraphInstance = GraphInstance;

            List<ProcGenGraphNodeView> nodeViews = new List<ProcGenGraphNodeView>();

            Group[] groups = new Group[graph.Meta.Groups.Length];

            for(int i = 0; i < graph.Meta.Groups.Length; ++i)
            {
                groups[i] = m_Provider.GraphView.CreateGroup(graph.Meta.Groups[i].Title);
                Color c = graph.Meta.Groups[i].Color;
                groups[i].style.opacity = c.a;
                c.a = 1f;
                groups[i].style.backgroundColor = c;
                m_Provider.GraphView.AddElement(groups[i]);
            }

            for (int i = 0; i < GraphInstance.Runtime.Nodes.Length; ++i)
            {
                int groupIdx = graph.Meta.Nodes[i].GroupIndex;
                ProcGenGraphNodeView nodeView = new ProcGenGraphNodeView(GraphInstance.Runtime.Nodes[i]);
                nodeView.DataUpdate = NotifyGraphChange;
                nodeViews.Add(nodeView);
                nodeView.SetPosition(graph.Meta.Nodes[i].Position);
                if (groupIdx != -1 )
                {
                    groups[groupIdx].AddElement(nodeView);
                    nodeView.ParentGroup = groups[groupIdx];
                }
                m_Provider.GraphView.AddElement(nodeView);
            }

            for (int i = 0; i < nodeViews.Count; ++i)
            {
                ProcGenGraphNodeView nodeView = nodeViews[i];
                for (int j = 0; j < nodeView.Node.Inputs.Length; ++j)
                {
                    ref NodeInput input = ref nodeView.Node.Inputs[j];
                    if (input.IsConnectorValid())
                    {
                        int outputIndex = System.Array.IndexOf(GraphInstance.Runtime.Nodes, input.Source);
                        if (nodeView.TryGetPort(j, false, out Port inputPort)
                            && nodeViews[outputIndex].TryGetPort(input.SourceOutputIndex, true, out Port outputPort))
                        {
                            var edge = inputPort.ConnectTo(outputPort);
                            m_Provider.GraphView.Add(edge);
                        }
                        else
                        {
                            Debug.LogError($"Failed to reconnect edges {i} -> {outputIndex}");
                        }
                    }
                }
            }
        }

        internal void NotifyGraphChange()
        {
            if (m_GraphDebugger != null)
            {
                m_GraphDebugger.NotifyGraphChanged();
            }
        }

        private void SaveGenerativeGraph()
        {
            GraphInstance.Graph.SerializeGraph(GraphInstance.Runtime, ProcGenSerialization.SerializationSettings);
            GraphInstance.Graph.Meta = m_Provider.GraphView.SerializeGraphMetadata(GraphInstance.Runtime);
            EditorUtility.SetDirty(GraphInstance.Graph);
        }


        private void OpenGenerativeGraph()
        {
            string filePath = EditorUtility.OpenFilePanel("Graph", Application.dataPath, "asset");
            if (filePath.StartsWith(Application.dataPath))
                filePath = filePath.Substring(Application.dataPath.Length - "Assets".Length);

            if (!string.IsNullOrEmpty(filePath))
            {
                GenerativeGraph graphAsset = AssetDatabase.LoadAssetAtPath<GenerativeGraph>(filePath);
                if (graphAsset != null)
                {
                    QuickLoadGraph(graphAsset);
                }
            }
        }

        private void ToggleInspector()
        {
            ElementInspector.Container.style.display = (DisplayStyle) (1 - (int) (ElementInspector.Container.style.display.value));
        }


        private void OnKey(KeyDownEvent keyEvent)
        {
            if (keyEvent.keyCode == KeyCode.Tab)
            {
                Vector2 referencePosition;
                referencePosition = keyEvent.imguiEvent.mousePosition;
                Vector2 screenPoint = this.position.position + referencePosition;
                DisplayContextMenu(screenPoint);
            }
        }

        private void DisplayContextMenu(Vector2 position)
        {
            SearchWindow.Open(new SearchWindowContext(position, 200f, 0.0f), m_Provider);
        }
    }
}