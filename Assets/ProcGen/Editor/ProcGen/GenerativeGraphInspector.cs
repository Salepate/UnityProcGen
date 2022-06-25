using ProcGen;
using UnityEditor;
using UnityEngine;

namespace ProcGenEditor
{
    [CustomEditor(typeof(GenerativeGraph))]
    public class GenerativeGraphInspector : Editor
    {
        private static readonly string[] m_IgnoredFields = new string[] { "m_Script" };
        private const string DebugGraphPref = "procgen.debuggraph";
        private bool m_Debug;

        private void OnEnable()
        {
            m_Debug = EditorPrefs.GetBool(DebugGraphPref, false);
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool(DebugGraphPref, m_Debug);
        }
        public override void OnInspectorGUI()
        {
            m_Debug = EditorGUILayout.Toggle("Debug Graph", m_Debug);
            if ( m_Debug )
            {
                DrawPropertiesExcluding(serializedObject, m_IgnoredFields);
            }
            else
            {
                EditorGUILayout.HelpBox(FormatHelpBox(), MessageType.Info);
            }

            if ( GUILayout.Button("Open Editor") )
            {
                OpenGraphEditor();
            }
        }

        private string FormatHelpBox()
        {
            GenerativeGraph graph = (GenerativeGraph)target;
            string information = string.Format("Nodes: {0}", graph.NodeTypes.Length);
            return information;
        }

        private void OpenGraphEditor()
        {
            ProcGenGraphEditor.QuickLoadGraph((GenerativeGraph)target);
        }
    }
}