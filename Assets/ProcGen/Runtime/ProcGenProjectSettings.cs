using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenEditor
{
    public class ProcGenProjectSettings : ScriptableObject
    {
        public const string FileName = "ProcGenSettings";

        public List<string> AdditionalAssemblies;

        public static ProcGenProjectSettings LoadPreferences()
        {
            return Resources.Load<ProcGenProjectSettings>(FileName);
        }
    }
}