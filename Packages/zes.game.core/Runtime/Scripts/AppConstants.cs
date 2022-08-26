using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zes
{
    [CreateAssetMenu(fileName = "appconstants", menuName = "Zes/App Constants", order = 1)]
    public class AppConstants : ScriptableObject
    {
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
