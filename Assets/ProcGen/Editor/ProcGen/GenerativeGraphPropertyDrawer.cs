using Dirt.ProcGen;
using UnityEditor;
using UnityEngine;

namespace ProcGenEditor
{
    [CustomPropertyDrawer(typeof(GenerativeGraph))]
    public class GenerativeGraphPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect control = position;
            Rect editBtn = position;

            control.width -= 100;
            editBtn.x = control.max.x + 10f;
            editBtn.width = position.width - editBtn.x;
            EditorGUI.ObjectField(control, property, new GUIContent(fieldInfo.Name));

            bool guiState = GUI.enabled;
            GUI.enabled = property.objectReferenceValue != null;
            if ( GUI.Button(editBtn, "Edit"))
            {
                ProcGenGraphEditor.QuickLoadGraph((GenerativeGraph)property.objectReferenceValue);
            }
            GUI.enabled = guiState;
        }
    }
}