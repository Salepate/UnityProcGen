using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProcGenEditor
{
    public class ProcGenGraphElementInspector
    {
        private IMGUIContainer m_GUIContainer;

        private System.Action LambdaGUI;
        public VisualElement Container { get; private set; }
        public ProcGenGraphElementInspector(VisualElement inspectorContainer)
        {
            Container = inspectorContainer;
            m_GUIContainer = Container.Q<IMGUIContainer>();
            m_GUIContainer.onGUIHandler = DrawGUI;
        }


        public void InspectElement(GraphElement graphElement)
        {
            if ( graphElement is Group grp)
            {
                LambdaGUI = () => DrawGroupGUI(grp);
            }
            else if ( graphElement is ProcGenGraphNodeView node)
            {
                LambdaGUI = () => DrawNodeUI(node);
            }
        }

        private void DrawGUI()
        {
            GUILayout.Label("Graph Inspector", GUI.skin.label);
            GUILayout.BeginVertical(GUI.skin.box);
            LambdaGUI?.Invoke();
            GUILayout.EndVertical();
        }

        private void DrawGroupGUI(Group group)
        {
            GUILayout.Label("Title");
            group.title = EditorGUILayout.TextField(group.title);
            Color c = ProcGenEditorHelper.BackgroundToColor(group);
            GUILayout.Label("Background");
            EditorGUI.BeginChangeCheck();
            c = EditorGUILayout.ColorField(c);
            if ( EditorGUI.EndChangeCheck())
            {
                float alpha = c.a;
                c.a = 1f;
                group.style.backgroundColor = c;
                group.style.opacity = alpha;
            }
        }

        private void DrawNodeUI(ProcGenGraphNodeView nodeView)
        {
            GUILayout.Label(nodeView.title);
            nodeView.IMGUI.onGUIHandler?.Invoke();
        }
    }


    //
}