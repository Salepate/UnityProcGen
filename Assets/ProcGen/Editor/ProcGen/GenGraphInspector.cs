using Dirt.ProcGen;
using UnityEditor;
using UnityEngine;

namespace ProcGenEditor.ProcGen
{
    [CustomEditor(typeof(GenerativeGraph))]
    public class GenGraphInspector : Editor
    {
        public override void OnInspectorGUI()
        {
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