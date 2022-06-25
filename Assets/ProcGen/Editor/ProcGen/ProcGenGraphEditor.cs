using Dirt.Utility;
using ProcGen;
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
        private SearchWindowProvider m_Provider;


        [MenuItem("Window/General/Generative Graph Editor")]
        private static void ShowMenu()
        {
            ProcGenGraphEditor win = GetWindow<ProcGenGraphEditor>("Generative Graph");
            win.position = new Rect(win.position.position, new Vector2(800f, 600f));
            win.Show(true);
        }

        public static void QuickLoadGraph(GenerativeGraph graph)
        {
            ShowMenu();
            GetWindow<ProcGenGraphEditor>().LoadGenerativeGraph(new GenerativeGraphInstance() { Graph = graph });
        }

        public static void DebugRuntime(GenerativeGraphInstance graphInstance)
        {
            ShowMenu();
            GetWindow<ProcGenGraphEditor>().LoadGenerativeGraph(graphInstance);
        }

        private void OnEnable()
        {
            var vt = Resources.Load<VisualTreeAsset>("window-procgen");
            var template =  vt.CloneTree();
            rootVisualElement.Add(template);
            template.StretchToParentSize();
            m_Provider = ScriptableObject.CreateInstance<SearchWindowProvider>();
            m_Provider.Editor = this;
            VisualElement toolbar = rootVisualElement.Q<VisualElement>("editor-toolbar");
            toolbar.Q<Button>("button-save").clicked += SaveGenerativeGraph;
            toolbar.Q<Button>("button-open").clicked += OpenGenerativeGraph;
            LoadGenerativeGraph(new GenerativeGraphInstance() { Graph = ScriptableObject.CreateInstance<GenerativeGraph>() });
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
            rootVisualElement.Q("graphRoot").RegisterCallback<KeyDownEvent>(OnKey);

            m_Provider.GraphView = graphView;
            graphView.styleSheets.Add(style);
        }

        private void LoadGenerativeGraph(GenerativeGraphInstance graphInstance)
        {
            ResetGraph();
            GraphInstance = graphInstance;
            if ( GraphInstance.Runtime == null) 
                GraphInstance.Runtime = graphInstance.Graph.Deserialize(ProcGenSerialization.SerializationSettings, ProcGenSerialization.NodeConverter);

            GenerativeGraph graph = graphInstance.Graph;
            m_Provider.GraphView.GraphInstance = GraphInstance;

            List<ProcGenGraphNodeView> nodeViews = new List<ProcGenGraphNodeView>();

            for(int i = 0; i < GraphInstance.Runtime.Nodes.Length; ++i)
            {
                ProcGenGraphNodeView nodeView = new ProcGenGraphNodeView(GraphInstance.Runtime.Nodes[i]);
                nodeView.DataUpdate += () => GraphInstance.OnGraphUpdate?.Invoke();
                nodeViews.Add(nodeView);
                nodeView.SetPosition(graph.Meta[i].Position);
                m_Provider.GraphView.AddElement(nodeView);
            }

            for (int i = 0; i < nodeViews.Count; ++i)
            {
                ProcGenGraphNodeView nodeView = nodeViews[i];
                for(int j = 0; j < nodeView.Node.Inputs.Length; ++j)
                {
                    ref NodeConnector input = ref nodeView.Node.Inputs[j];
                    if ( input.IsConnectorValid())
                    {
                        int outputIndex = System.Array.IndexOf(GraphInstance.Runtime.Nodes, input.Source);
                        if ( nodeView.TryGetPort(j, false, out Port inputPort)
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

        private void SaveGenerativeGraph()
        {
            GraphInstance.Graph.SerializeGraph(GraphInstance.Runtime, ProcGenSerialization.SerializationSettings);
            GraphInstance.Graph.Meta = m_Provider.GraphView.SerializeNodeMeta(GraphInstance.Runtime);
            EditorUtility.SetDirty(GraphInstance.Graph);
        }

        private void OpenGenerativeGraph()
        {
            string filePath = EditorUtility.OpenFilePanel("Graph", Application.dataPath, "asset");
            if (filePath.StartsWith(Application.dataPath))
                filePath = filePath.Substring(Application.dataPath.Length - "Assets".Length);

            if ( !string.IsNullOrEmpty(filePath))
            {
                GenerativeGraph graphAsset = AssetDatabase.LoadAssetAtPath<GenerativeGraph>(filePath);
                if ( graphAsset != null)
                {
                    QuickLoadGraph(graphAsset);
                }
            }
        }

        private void OnKey(KeyDownEvent keyEvent)
        {
            if (keyEvent.keyCode == KeyCode.Tab )
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


        private class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
        {
            private string[] m_Assemblies;
            
            public ProcGenGraphView GraphView { get; set; }
            public VisualElement EditorRoot => Editor.rootVisualElement;
            public ProcGenGraphEditor Editor { get; set; }

            public SearchWindowProvider()
            {
                m_Assemblies = new string[1];
                m_Assemblies[0] = typeof(BaseNode).Assembly.FullName;
            }

            public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
            {
                var res = new List<SearchTreeEntry>();
                var nodes = AssemblyReflection.BuildTypeMap<BaseNode>(m_Assemblies);
                res.Add(new SearchTreeGroupEntry(new GUIContent("Create Node"), 0));
                foreach(var nodeEntry in nodes)
                {
                    if (nodeEntry.Value.IsAbstract)
                        continue;

                    res.Add(new SearchTreeEntry(new GUIContent(nodeEntry.Value.Name))
                    {
                        level = 1,
                        userData = nodeEntry.Value
                    });
                }
                return res;
            }

            public bool OnSelectEntry(SearchTreeEntry searchEntry, SearchWindowContext context)
            {
                BaseNode node = (BaseNode) CreateInstance((Type)searchEntry.userData);
                node.Initialize();
                ProcGenGraphNodeView nodeView = new ProcGenGraphNodeView(node);
                nodeView.DataUpdate += () => GraphView.GraphInstance.OnGraphUpdate?.Invoke();
                var windowMousePosition = EditorRoot.ChangeCoordinatesTo(EditorRoot.parent, context.screenMousePosition - Editor.position.position);
                var graphMousePosition = GraphView.contentViewContainer.WorldToLocal(windowMousePosition);
                ArrayUtility.Add(ref Editor.GraphInstance.Runtime.Nodes, node);
                GraphView.AddElement(nodeView);
                nodeView.SetOrigin(graphMousePosition);
                return true;
            }
        }
    }
}