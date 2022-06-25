using ProcGen;
using UnityEditor;
using UnityEngine;

namespace ProcGenEditor
{
    [CustomEditor(typeof(GenerativeGraph))]
    public class GenerativeGraphInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if ( GUILayout.Button("Open Editor") )
            {
                OpenGraphEditor();
            }
        }

        private void OpenGraphEditor()
        {
            ProcGenGraphEditor.QuickLoadGraph((GenerativeGraph)target);
        }
    }
}