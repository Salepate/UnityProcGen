using ProcGen.Connector;
using ProcGen.Nodes.Output;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProcGenEditor.NodeEditor
{
    [CustomEditor(typeof(GraphOutputNode))]
    public class GraphOutputNodeEditor : BaseNodeInspector
    {
        private string[] m_LayoutNames;

        protected override void OnEnable()
        {
            IsMainGUI = true;
            GraphOutputNode outputNode = (GraphOutputNode)target;
            m_LayoutNames = outputNode.LayoutNames.ToArray();
        }
        public override string[] InputNamesOverride => m_LayoutNames;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty bufferLayout = serializedObject.FindProperty("BufferLayout");
            SerializedProperty layoutNames = serializedObject.FindProperty("LayoutNames");
            bool redrawView = false;

            int sizeDt = layoutNames.arraySize - bufferLayout.arraySize;

            for (int i = 0; sizeDt < 0 && i < -sizeDt; ++i)
            {
                layoutNames.InsertArrayElementAtIndex(layoutNames.arraySize);
                redrawView = true;
            }
            for (int i = sizeDt; i > 0; --i)
            {
                layoutNames.DeleteArrayElementAtIndex(layoutNames.arraySize - 1);
                redrawView = true;
            }

            for (int i = 0; i < bufferLayout.arraySize && !redrawView; ++i)
            {
                UserAction action = DrawEntry(bufferLayout, layoutNames, i);
                redrawView |= HandleUserAction(action, bufferLayout, layoutNames, i);
            }

            if ( GUILayout.Button("+"))
            {
                redrawView |= HandleUserAction(UserAction.Insert, bufferLayout, layoutNames, bufferLayout.arraySize);
            }

            if (redrawView)
            {
                serializedObject.ApplyModifiedProperties();
                GraphOutputNode outputNode = (GraphOutputNode)target;
                m_LayoutNames = outputNode.LayoutNames.ToArray();
                RecreatePorts();
            }
        }

        private bool HandleUserAction(UserAction action, SerializedProperty bufferProp, SerializedProperty layoutProp, int entryIndex)
        {
            switch (action)
            {
                case UserAction.Insert:
                    bufferProp.InsertArrayElementAtIndex(entryIndex);
                    layoutProp.InsertArrayElementAtIndex(entryIndex);
                    break;
                case UserAction.Delete:
                    bufferProp.DeleteArrayElementAtIndex(entryIndex);
                    layoutProp.DeleteArrayElementAtIndex(entryIndex);
                    break;
                case UserAction.UpdatePort:
                    break;
                case UserAction.None:
                default:
                    return false;
            }
            return true;
        }


        private UserAction DrawEntry(SerializedProperty bufferLayout, SerializedProperty layoutNames, int entryIndex)
        {
            UserAction action = UserAction.None;

            SerializedProperty bufferProp = bufferLayout.GetArrayElementAtIndex(entryIndex);
            SerializedProperty layoutProp = layoutNames.GetArrayElementAtIndex(entryIndex);
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            int enumValue = EditorGUILayout.Popup(bufferProp.enumValueIndex, bufferProp.enumDisplayNames, GUILayout.Width(100f));
            if ( EditorGUI.EndChangeCheck())
            {
                bufferProp.enumValueIndex = enumValue;
                action = UserAction.UpdatePort;
            }
            EditorGUI.BeginChangeCheck();

            string label = EditorGUILayout.TextField(layoutProp.stringValue, GUILayout.Width(150f));
            if ( EditorGUI.EndChangeCheck() )
            {
                layoutProp.stringValue = label;
                action = UserAction.UpdatePort;
            }

            if (GUILayout.Button("+"))
                action = UserAction.Insert;
            if (GUILayout.Button("-"))
                action = UserAction.Delete;

            GUILayout.EndHorizontal();

            return action;
        }



        internal enum UserAction
        {
            None,
            Insert,
            Delete,
            UpdatePort,
        }
    }
}