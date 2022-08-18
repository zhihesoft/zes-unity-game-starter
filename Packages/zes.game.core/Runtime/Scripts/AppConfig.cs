using UnityEngine;

namespace Zes
{
    [CreateAssetMenu(fileName = "app", menuName = "ZES/AppConfig", order = 1)]
    public class AppConfig : ScriptableObject
    {
        public string patchDataPath = "patch_data";

        [Header("Javascript Editor Settings")]
        public string javascriptPathEditor = "Typescripts";
        public string javascriptEntryEditor = "./dist/index.cjs";

        [Header("Javascript Runtime Settings")]
        public string javascriptBundle = "texts";
        public string javascriptPathRuntime = "Assets/Bundles/texts/main.cjs";
        public string javascriptEntryRuntime = "main";
    }
}
