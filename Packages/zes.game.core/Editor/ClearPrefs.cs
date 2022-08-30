using UnityEditor;
using UnityEngine;

namespace Zes
{
    public static class ClearPrefs
    {
        [MenuItem("ZES/Util/Clear PlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log($"player prefs cleared");
        }
    }
}

