using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProcGenEditor
{
    public static class ProcGenEditorHelper
    {
        public static string FormatNodeName(string nodeName)
        {
            if (nodeName.EndsWith("Node"))
                nodeName = nodeName.Substring(0, nodeName.Length - 4);
            return ObjectNames.NicifyVariableName(nodeName);
        }

        public static Color BackgroundToColor(VisualElement elem)
        {
            Color c = Color.black;
            if (elem.style.backgroundColor != null)
                c = elem.style.backgroundColor.value;
            if (elem.style.opacity != null)
                c.a = elem.style.opacity.value;
            return c;
        }
    }
}