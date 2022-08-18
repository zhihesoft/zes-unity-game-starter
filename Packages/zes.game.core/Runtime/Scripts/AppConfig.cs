using UnityEngine;

namespace Zes
{
    [CreateAssetMenu(fileName = "app", menuName = "ZES/AppConfig", order = 1)]
    public class AppConfig : ScriptableObject
    {
        public int maxCachedAsset = 512;

        public string patchDataPath = "patch_data";

        [Header("Javascript Editor Settings")]
        public string javascriptPathEditor = "Typescripts";
        public string javascriptEntryEditor = "./dist/index.js";

        [Header("Javascript Runtime Settings")]
        public string javascriptBundle = "texts";
        public string javascriptPathRuntime = "Assets/Bundles/texts/main.bytes";
        public string javascriptEntryRuntime = "main";
    }
}
