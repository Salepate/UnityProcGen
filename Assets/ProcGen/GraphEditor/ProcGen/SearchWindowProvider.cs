using Dirt.Utility;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProcGenEditor
{
    internal class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private string[] m_Assemblies;
            
        public ProcGenGraphView GraphView { get; set; }
        public VisualElement EditorRoot => Editor.rootVisualElement;
        public ProcGenGraphEditor Editor { get; set; }

        public SearchWindowProvider()
        {

        }

        private void OnEnable()
        {
            List<string> assemblies = new List<string>();
            assemblies.Add(typeof(BaseNode).Assembly.FullName);

            if (ProcGenPreferences.Exists)
            {
                ProcGenProjectSettings settings = ProcGenPreferences.Settings;
                //TODO: check assembly validity
                assemblies.AddRange(settings.AdditionalAssemblies);
                assemblies.RemoveAll(string.IsNullOrEmpty);
            }
            m_Assemblies = assemblies.ToArray();
        }

        private class SearchTreeContainer
        {
            public string ContainerName;
            public List<Tuple<string, Type>> Nodes;

            public SearchTreeContainer(string containerName)
            {
                ContainerName = containerName;
                Nodes = new List<Tuple<string, Type>>();
            }
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var res = new List<SearchTreeEntry>();
            var nodes = AssemblyReflection.BuildTypeMap<BaseNode>(m_Assemblies);
            res.Add(new SearchTreeGroupEntry(new GUIContent("Create Node"), 0));

            Dictionary<string, SearchTreeContainer> containers = new Dictionary<string, SearchTreeContainer>();

            // build groups
            foreach(var nodeEntry in nodes)
            {
                if (nodeEntry.Value.IsAbstract)
                    continue;

                string[] namespaces = nodeEntry.Value.FullName.Split('.');
                string sub = "Default";

                if (namespaces.Length > 1)
                    sub = namespaces[namespaces.Length - 2];

                if (!containers.TryGetValue(sub, out SearchTreeContainer container))
                {
                    container = new SearchTreeContainer(sub);
                    containers.Add(sub, container);
                }
                container.Nodes.Add(Tuple.Create(nodeEntry.Value.Name, nodeEntry.Value));


            }

            var sortedGroups = containers.Values.ToList();
            sortedGroups.Sort((a, b) => a.ContainerName.CompareTo(b.ContainerName));
            for(int i = 0; i < sortedGroups.Count; ++i)
            {
                sortedGroups[i].Nodes.Sort((a, b) => a.Item1.CompareTo(b.Item1));
                res.Add(new SearchTreeGroupEntry(new GUIContent(sortedGroups[i].ContainerName), 1));
                for(int j = 0; j < sortedGroups[i].Nodes.Count; ++j)
                {
                    var nodeEntry = sortedGroups[i].Nodes[j];

                    res.Add(new SearchTreeEntry(new GUIContent(ProcGenEditorHelper.FormatNodeName(nodeEntry.Item1)))
                    {
                        level = 2,
                        userData = nodeEntry.Item2
                    });
                }
            }

            return res;
        }

        public bool OnSelectEntry(SearchTreeEntry searchEntry, SearchWindowContext context)
        {
            BaseNode node = (BaseNode) CreateInstance((Type)searchEntry.userData);
            node.Initialize();
            node.SetIndex(Editor.GraphInstance.Runtime.Nodes.Length);
            ProcGenGraphNodeView nodeView = new ProcGenGraphNodeView(node);
            nodeView.DataUpdate = Editor.NotifyGraphChange;
            var windowMousePosition = EditorRoot.ChangeCoordinatesTo(EditorRoot.parent, context.screenMousePosition - Editor.position.position);
            var graphMousePosition = GraphView.contentViewContainer.WorldToLocal(windowMousePosition);
            ArrayUtility.Add(ref Editor.GraphInstance.Runtime.Nodes, node);
            GraphView.AddElement(nodeView);
            nodeView.SetOrigin(graphMousePosition);
            return true;
        }
    }
}