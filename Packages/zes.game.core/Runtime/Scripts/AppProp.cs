using UnityEngine;

namespace Zes
{
    [CreateAssetMenu(fileName = "app", menuName = "ZES/App property", order = 1)]
    public class AppProp : ScriptableObject
    {
        /// <summary>
        /// patch data path in device
        /// </summary>
        public string patchDataPath = "patch_data";

        /// <summary>
        /// Typescript project path, relative to project root dir
        /// </summary>
        public string typescriptProjectPath = "Typescripts";

        /// <summary>
        /// javascript entry for editor
        /// </summary>
        public string javascriptEntryEditor = "./dist/index.js";

        /// <summary>
        /// javascript entry for runtime
        /// </summary>
        public string javascriptBundle = "text";

        /// <summary>
        /// javascript entry for runtime
        /// </summary>
        public string javascriptEntryRuntime = "Assets/Bundles/texts/main.bytes";

    }
}
