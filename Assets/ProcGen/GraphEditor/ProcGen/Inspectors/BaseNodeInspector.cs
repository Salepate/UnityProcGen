using ProcGen;
using UnityEditor;
using UnityEngine;

namespace ProcGenEditor
{
    [CustomEditor(typeof(BaseNode), true)]
    public class BaseNodeInspector : Editor
    {
        public const float Padding = 5f;
        private static readonly string[] m_IgnoredProperties = new string[] { "m_Script" };

        public System.Action OnPortUpdate;
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(Padding);
            GUILayout.BeginVertical();
            GUILayout.Space(Padding);
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, m_IgnoredProperties);
            serializedObject.ApplyModifiedProperties();
            GUILayout.Space(Padding);
            GUILayout.EndVertical();
            GUILayout.Space(Padding);
            GUILayout.EndHorizontal();
        }

        protected void RecreatePorts() => OnPortUpdate?.Invoke();
    }
}