using UnityEditor;

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
    }
}