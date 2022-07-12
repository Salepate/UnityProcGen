using ProcGen.Connector;
using ProcGen.Nodes.Output;
using UnityEditor;
using UnityEngine;

namespace ProcGenEditor.NodeEditor
{
    [CustomEditor(typeof(GraphOutputNode))]
    public class GraphOutputNodeEditor : BaseNodeInspector
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            bool redrawView = false;
            var contentProp = serializedObject.FindProperty("BufferContent");

            for(int i = 0; i < contentProp.arraySize; ++i)
            {
                UserAction action = DrawEntry(contentProp.GetArrayElementAtIndex(i), i);
                redrawView |= HandleUserAction(action, contentProp, i);
            }

            if ( GUILayout.Button("+"))
            {
                redrawView |= HandleUserAction(UserAction.Insert, contentProp, contentProp.arraySize);
            }

            if (redrawView)
            {
                serializedObject.ApplyModifiedProperties();
                RecreatePorts();
            }
        }

        private bool HandleUserAction(UserAction action, SerializedProperty bufferProp, int entryIndex)
        {
            switch (action)
            {
                case UserAction.Insert:
                    bufferProp.InsertArrayElementAtIndex(entryIndex);
                    break;
                case UserAction.Delete:
                    bufferProp.DeleteArrayElementAtIndex(entryIndex);
                    break;
                case UserAction.Update:
                    break;
                case UserAction.None:
                default:
                    return false;
            }
            return true;
        }


        private UserAction DrawEntry(SerializedProperty prop, int entryIndex)
        {
            UserAction action = UserAction.None;

            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            int enumValue = EditorGUILayout.Popup(prop.enumValueIndex, prop.enumDisplayNames);
            if ( EditorGUI.EndChangeCheck())
            {
                prop.enumValueIndex = enumValue;
                action = UserAction.Update;
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
            Update
        }
    }
}