using UnityEngine;

namespace Zes
{
    [CreateAssetMenu(fileName = "appconstants", menuName = "Zes/App Constants", order = 1)]
    public class AppConstants : ScriptableObject
    {
        public string shortName = "game";

        [Header("i18n")]
        public int languageStartId = 18000;
        public string languageConfigName = "language";

        /// <summary>
        /// bundles settings
        /// </summary>
        [Header("Bundles")]
        public string bundleOutputPath = "AssetBundles";

        /// <summary>
        /// config bundle path
        /// </summary>
        public string configurationBundlePath = "Assets/Bundles/conf";

        /// <summary>
        /// patch data path in device
        /// </summary>
        [Header("Patch")]
        public string patchDataPath = "patch_data";

        /// <summary>
        /// patch info file name
        /// </summary>
        public string patchInfoFile = "patch.json";

        /// <summary>
        /// version info file name
        /// </summary>
        public string versionInfoFile = "version.json";

        /// <summary>
        /// Typescript project path, relative to project root dir
        /// </summary>
        [Header("Script")]
        public string javascriptProjectPath = "Typescripts";

        /// <summary>
        /// javascript entry for editor
        /// </summary>
        public string javascriptEntryEditor = "./dist/index.js";

        /// <summary>
        /// javascript entry for editor
        /// </summary>
        public string javascriptBuildResult = "./out/main.bytes";

        /// <summary>
        /// javascript entry for runtime
        /// </summary>
        public string javascriptBundle = "text";

        /// <summary>
        /// javascript entry for runtime
        /// </summary>
        public string javascriptEntryRuntime = "Assets/Bundles/texts/main.bytes";

        /// <summary>
        /// Output dir
        /// </summary>
        [Header("Output")]
        public string outputDir = "out";

        public string javascriptEntry
        {
            get
            {
#if UNITY_EDITOR
                return javascriptEntryEditor;
#else
                return javascriptEntryRuntime;
#endif
            }
        }

    }
}
