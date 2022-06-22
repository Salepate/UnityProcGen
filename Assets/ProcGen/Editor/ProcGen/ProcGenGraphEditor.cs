using Dirt.ProcGen;
using Dirt.Utility;
using Newtonsoft.Json;
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
        public RuntimeGraph GraphInstance { get; private set; }
        private SearchWindowProvider m_Provider;
        private GenerativeGraph m_ActiveGraph;
        private JsonSerializerSettings m_SerializationSettings;

        [MenuItem("Proc Gen/Graph Editor")]
        private static void ShowMenu()
        {
            ProcGenGraphEditor win = GetWindow<ProcGenGraphEditor>();
            win.position = new Rect(win.position.position, new Vector2(800f, 600f));
            win.Show(true);

            win.LoadGenerativeGraph(ScriptableObject.CreateInstance<GenerativeGraph>());
        }

        public static void QuickLoadGraph(GenerativeGraph graph)
        {
            ShowMenu();
            EditorWindow.GetWindow<ProcGenGraphEditor>().LoadGenerativeGraph(graph);
        }

        private void OnEnable()
        {
            var vt = Resources.Load<VisualTreeAsset>("window-procgen");
            var style = Resources.Load<StyleSheet>("procgen-graph-style");
            var template =  vt.CloneTree();
            rootVisualElement.Add(template);
            template.StretchToParentSize();

            ProcGenGraphView graphView = new ProcGenGraphView();
            template.Q("graphRoot").Add(graphView);
            graphView.StretchToParentSize();
            graphView.AddManipulator(new ContentDragger());
            graphView.AddManipulator(new SelectionDragger());
            graphView.AddManipulator(new ClickSelector());
            template.RegisterCallback<KeyDownEvent>(OnKey);

            m_Provider = ScriptableObject.CreateInstance<SearchWindowProvider>();
            m_Provider.Graph = graphView;
            m_Provider.Editor = this;
            graphView.styleSheets.Add(style);

            VisualElement toolbar = rootVisualElement.Q<VisualElement>("editor-toolbar");
            toolbar.Q<Button>("button-save").clicked += SaveGenerativeGraph;

            // serial
            m_SerializationSettings = new JsonSerializerSettings()
            {
                ContractResolver = new ProcGenContractResolver()
            };

            m_SerializationSettings.Converters.Add(new VectorConverter());
        }

        private void LoadGenerativeGraph(GenerativeGraph graph)
        {
            m_ActiveGraph = graph;
            GraphInstance = m_ActiveGraph.Deserialize(ProcGenSerialization.SerializationSettings, ProcGenSerialization.NodeConverter);
            m_Provider.Graph.GraphInstance = GraphInstance;

            List<ProcGenGraphNodeView> nodeViews = new List<ProcGenGraphNodeView>();

            for(int i = 0; i < GraphInstance.Nodes.Length; ++i)
            {
                ProcGenGraphNodeView nodeView = new ProcGenGraphNodeView(GraphInstance.Nodes[i]);
                nodeViews.Add(nodeView);
                m_Provider.Graph.AddElement(nodeView);
            }

            for (int i = 0; i < nodeViews.Count; ++i)
            {
                ProcGenGraphNodeView nodeView = nodeViews[i];
                for(int j = 0; j < nodeView.Node.Inputs.Length; ++j)
                {
                    ref NodeConnector input = ref nodeView.Node.Inputs[i];
                    if ( input.IsConnectorValid())
                    {
                        int outputIndex = System.Array.IndexOf(GraphInstance.Nodes, input.Source);
                        if ( nodeView.TryGetPort(j, false, out Port inputPort)
                            && nodeViews[outputIndex].TryGetPort(input.SourceOutputIndex, true, out Port outputPort))
                        {
                            var edge = inputPort.ConnectTo(outputPort);
                            m_Provider.Graph.Add(edge);
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
            m_ActiveGraph.SerializeGraph(GraphInstance, m_SerializationSettings);
            EditorUtility.SetDirty(m_ActiveGraph);
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
            
            public ProcGenGraphView Graph { get; set; }
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
                BaseNode node = (BaseNode) System.Activator.CreateInstance((Type)searchEntry.userData);
                node.Initialize();
                ProcGenGraphNodeView nodeView = new ProcGenGraphNodeView(node);
                var windowMousePosition = EditorRoot.ChangeCoordinatesTo(EditorRoot.parent, context.screenMousePosition - Editor.position.position);
                var graphMousePosition = Graph.contentViewContainer.WorldToLocal(windowMousePosition);
                ArrayUtility.Add(ref Editor.GraphInstance.Nodes, node);
                Graph.AddElement(nodeView);
                nodeView.SetOrigin(graphMousePosition);
                return true;
            }
        }
    }
}