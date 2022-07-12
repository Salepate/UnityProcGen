using ProcGen.Nodes.Constants;
using UnityEditor;
using UnityEngine;

namespace ProcGenEditor.NodeEditor
{
    public class ConstantNodeEditor : BaseNodeInspector
    {
        public virtual float Width => 60f;
        public virtual string Label => "v";
        protected override void OnEnable()
        {
            base.OnEnable();
            IsMainGUI = true;
        }

        public override void OnInspectorGUI()
        {
            float lw = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 10f;
            GUILayout.BeginHorizontal();
            GUILayout.Space(5f);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Value"), new GUIContent(Label), GUILayout.Width(Width));
            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = lw;
        }
    }

    [CustomEditor(typeof(FloatNode))]
    public class FloatNodeEditor : ConstantNodeEditor { }

    [CustomEditor(typeof(IntNode))]
    public class IntNodeEditor : ConstantNodeEditor { }

    [CustomEditor(typeof(Vector2Node))]
    public class Vector2NodeEditor : ConstantNodeEditor
    {
        public override float Width => 90f;
        public override string Label => string.Empty;
    }

    [CustomEditor(typeof(Vector3Node))]
    public class Vector3NodeEditor : ConstantNodeEditor
    {
        public override float Width => 110f;
        public override string Label => string.Empty;
    }
}