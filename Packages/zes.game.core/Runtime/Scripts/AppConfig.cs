﻿using System;

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
        public string appShortName = "game";

        /// <summary>
        /// App display name
        /// </summary>
        public string appDisplayName = "game full name";

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


    }
}
