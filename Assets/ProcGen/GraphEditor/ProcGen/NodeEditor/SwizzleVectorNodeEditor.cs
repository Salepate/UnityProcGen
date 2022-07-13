using ProcGen.Nodes.Composites;
using UnityEditor;
using UnityEngine;

namespace ProcGenEditor.NodeEditor
{
    [CustomEditor(typeof(SwizzleVectorNode))]
    public class SwizzleVectorNodeEditor : BaseNodeInspector
    {
        private SerializedProperty[] m_PropArray;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_PropArray == null)
            {
                m_PropArray = new SerializedProperty[3];
            }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            m_PropArray[0] = serializedObject.FindProperty("X");
            m_PropArray[1] = serializedObject.FindProperty("Y");
            m_PropArray[2] = serializedObject.FindProperty("Z");


            GUILayout.Space(5f);
            for(int i = 0; i < m_PropArray.Length; ++i)
            {
                SerializedProperty p = m_PropArray[i];
                GUILayout.BeginHorizontal();
                GUILayout.Space(5f);
                EditorGUI.BeginChangeCheck();
                int newValue = (int) (SwizzleVectorNode.Axis) EditorGUILayout.EnumPopup((SwizzleVectorNode.Axis)p.enumValueIndex, GUILayout.Width(40f));
                if ( EditorGUI.EndChangeCheck())
                {
                    p.enumValueIndex = newValue;
                }
                GUILayout.FlexibleSpace();
                GUILayout.Label(" > ", GUILayout.Width(20f));
                GUILayout.FlexibleSpace();
                GUILayout.Label(p.displayName, GUILayout.Width(20f));
                GUILayout.Space(5f);
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(5f);

            serializedObject.ApplyModifiedProperties();
        }
    }
}