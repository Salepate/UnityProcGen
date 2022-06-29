using ProcGen;
using UnityEditor;
using UnityEngine;

namespace ProcGenEditor
{
    [CustomPropertyDrawer(typeof(GenerativeGraphInstance))]
    public class GenerativeGraphInstancePropertyDrawer : PropertyDrawer
    {
        private const string EditMessage = "Edit";
        private const string DebugMessage = "Debug";

        public override void OnGUI(Rect position, SerializedProperty graphInstanceProperty, GUIContent label)
        {
            
            Rect control = position;
            Rect editBtn = position;
            SerializedProperty property = graphInstanceProperty.FindPropertyRelative("Graph");

            control.width -= 100;
            editBtn.x = control.max.x + 10f;
            editBtn.width = position.width - editBtn.x;
            EditorGUI.ObjectField(control, property, new GUIContent(fieldInfo.Name));

            bool guiState = GUI.enabled;
            GUI.enabled = property.objectReferenceValue != null;
            if ( GUI.Button(editBtn, Application.isPlaying ? DebugMessage : EditMessage))
            {
                Object declaringObj = graphInstanceProperty.serializedObject.targetObject;
                GenerativeGraphInstance graphInstance = (GenerativeGraphInstance)(declaringObj.GetType().GetField(fieldInfo.Name).GetValue(declaringObj));
                EditAction(graphInstance);
            }
            GUI.enabled = guiState;
        }

        private void EditAction(GenerativeGraphInstance graphInstance)
        {
            if ( Application.isPlaying )
            {
                ProcGenGraphEditor.DebugRuntime(graphInstance);
            }
            else
            {
                ProcGenGraphEditor.QuickLoadGraph(graphInstance.Graph);
            }
        }
    }
}