using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProcGenEditor
{
    public static class ProcGenPreferences
    {
        public static readonly string DefaultSettingsPath = $"Assets/{ProcGenProjectSettings.FileName}.asset";
        private static readonly string AssetFilter = $"t:{nameof(ProcGenProjectSettings)}";
        private static ProcGenProjectSettings m_Settings = null;

        internal static bool Exists => AssetDatabase.FindAssets(AssetFilter).Length > 0;

        internal static ProcGenProjectSettings Settings
        {
            get
            {
                if (m_Settings != null)
                    return m_Settings;

                m_Settings = ForceLoad();

                if (m_Settings == null)
                {
                    m_Settings = ScriptableObject.CreateInstance<ProcGenProjectSettings>();
                    AssetDatabase.CreateAsset(m_Settings, DefaultSettingsPath);
                    AssetDatabase.SaveAssets();
                }

                return m_Settings;
            }
        }

        internal static ProcGenProjectSettings ForceLoad()
        {
            string[] guids = AssetDatabase.FindAssets(AssetFilter);
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                m_Settings = AssetDatabase.LoadAssetAtPath<ProcGenProjectSettings>(path);
                return m_Settings;
            }
            return null;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(Settings);
        }
    }

    internal static class ProcGenPreferencesUI
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var rawSettings = ProcGenPreferences.ForceLoad();
            if (rawSettings == null)
                rawSettings = ScriptableObject.CreateInstance<ProcGenProjectSettings>();

            var provider = new SettingsProvider("Project/Proc Gen", SettingsScope.Project)
            {
                label = "Proc Gen",
                guiHandler = (searchContext) =>
                {
                    var settings = ProcGenPreferences.Settings;
                    SerializedObject serializedObj = new SerializedObject(settings);
                    EditorGUI.BeginChangeCheck();

                    serializedObj.Update();

                    var assemblies = serializedObj.FindProperty("AdditionalAssemblies");

                    EditorGUILayout.PropertyField(assemblies, new GUIContent("Assemblies"));

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObj.ApplyModifiedProperties();
                        EditorUtility.SetDirty(settings);
                    }
                },
                keywords = new HashSet<string>(new string[] { "Proc", "Generative", "Generation" }),
                deactivateHandler = () =>
                {
                    AssetDatabase.SaveAssets();
                },
            };
            return provider;
        }
    }
}
