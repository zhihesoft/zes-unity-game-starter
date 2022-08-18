using UnityEditor;
using UnityEngine;

namespace ZEditor
{
    public class ClearPrefs
    {
        [MenuItem("SKIN/Util/Clear PlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log($"player prefs cleared");
        }
    }
}

