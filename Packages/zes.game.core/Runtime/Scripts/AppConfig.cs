﻿using System;
using UnityEngine;

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
        /// patch data path in device
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
