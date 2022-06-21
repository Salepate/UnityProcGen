using Dirt.ProcGen;
using Dirt.Utility;
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
        private SearchWindowProvider m_Provider;

        [MenuItem("Proc Gen/Graph Editor")]
        private static void ShowMenu()
        {
            ProcGenGraphEditor win = GetWindow<ProcGenGraphEditor>();
            win.position = new Rect(win.position.position, new Vector2(800f, 600f));
            win.Show(true);
        }

        public static void QuickLoadGraph(GenGraph graph)
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
        }

        private void LoadGenerativeGraph(GenGraph graph)
        {
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
            
            public GraphView Graph { get; set; }
            public VisualElement EditorRoot => Editor.rootVisualElement;
            public EditorWindow Editor { get; set; }

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
                ProcGenGraphNodeView nodeView = new ProcGenGraphNodeView(node);

                var windowMousePosition = EditorRoot.ChangeCoordinatesTo(EditorRoot.parent, context.screenMousePosition - Editor.position.position);
                var graphMousePosition = Graph.contentViewContainer.WorldToLocal(windowMousePosition);
                Graph.AddElement(nodeView);
                nodeView.SetOrigin(graphMousePosition);
                return true;
            }
        }
    }
}