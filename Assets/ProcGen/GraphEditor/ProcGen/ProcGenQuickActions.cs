using ProcGen;
using UnityEditor;

namespace ProcGenEditor
{
    public static class ProcGenQuickAction
    {
        //[MenuItem("Proc Gen/Reserialize Graphs")]
        private static void ReserializeGraphs()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(GenerativeGraph)}");
            for(int i = 0; i < guids.Length; ++i)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                GenerativeGraph graph = AssetDatabase.LoadAssetAtPath<GenerativeGraph>(path);
                // write serial code here
                EditorUtility.SetDirty(graph);
            }
        }
    }
}