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
        /// App short name
        /// </summary>
        public string appName = "game";

        /// <summary>
        /// 
        /// App language
        /// </summary>
        public string appLanguage = "zh-cn";

        /// <summary>
        /// Game login server
        /// </summary>
        public string loginServer = "";

        /// <summary>
        /// Allow guest login
        /// </summary>
        public bool allowGuest = false;

        /// <summary>
        /// Patch server 
        /// </summary>
        public string patchServer = "";

        /// <summary>
        /// Mininmun version
        /// </summary>
        public string minVersion = "";

        /// <summary>
        /// if check update
        /// </summary>
        public bool checkUpdate = true;

        /// <summary>
        /// patch data dir
        /// </summary>
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
        /// javascript entry for runtime
        /// </summary>
        public string javascriptBundle = "js";

        /// <summary>
        /// javascript entry for runtime
        /// </summary>
        public string javascriptData = "Assets/Bundles/js/main.bytes";

        /// <summary>
        /// javascript entry for runtime (only for editor, runtime entry is fixed)
        /// </summary>
        public string javascriptEntry = "Typescripts/dist/index.js";

        /// <summary>
        /// config bundle name
        /// </summary>
        public string configBundleName = "conf";

        /// <summary>
        /// config bundle path
        /// </summary>
        public string configBundlePath = "Assets/Bundles/conf";

        /// <summary>
        /// language bundle name
        /// </summary>
        public string languageBundleName = "language";

        /// <summary>
        /// language bundle path
        /// </summary>
        public string languageBundlePath = "Assets/Bundles/language";

    }
}
