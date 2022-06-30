using ProcGen.Utils;
using UnityEditor;
using UnityEngine;

namespace ProcGenEditor
{
    [CustomPropertyDrawer(typeof(ComparisonEnumAttribute))]
    public class ComparisonPropertyDrawer : PropertyDrawer
    {
        private static readonly GUIContent[] m_DropDown = new GUIContent[]
        {
            new GUIContent("=="),
            new GUIContent("!="),
            new GUIContent(">"),
            new GUIContent(">="),
            new GUIContent("<"),
            new GUIContent("<=")
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.width = 50f;
            EditorGUI.BeginChangeCheck();
            int selected = EditorGUI.Popup(position, property.enumValueIndex, m_DropDown);
            if ( EditorGUI.EndChangeCheck())
            {
                property.enumValueIndex = selected;
            }
        }
    }
}