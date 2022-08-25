using System;

namespace Zes
{
    [Serializable]
    public class AppConfig
    {
        /// <summary>
        /// Config name
        /// </summary>
        public string name = "";

        /// <summary>
        /// Config description
        /// </summary>
        public string description = "";

        /// <summary>
        /// App language
        /// </summary>
        public string appLanguage = "zh-cn";

        /// <summary>
        /// patch data path in device
        /// </summary>
        public string patchDataPath = "patch_data";

        /// <summary>
        /// javascript entry for editor
        /// </summary>
        public string javascriptEntryEditor = "./dist/index.js";

        /// <summary>
        /// javascript entry for runtime
        /// </summary>
        public string javascriptEntryRuntime = "text:Assets/Bundles/texts/main.bytes";
    }
}
