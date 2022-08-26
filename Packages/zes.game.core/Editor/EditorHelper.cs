using System.IO;
using UnityEditor;
using UnityEngine;
using Zes.Settings;

namespace Zes
{
    public static class EditorHelper
    {
        public static AppConstants LoadAppConstants()
        {
            return AssetDatabase.LoadAssetAtPath<AppConstants>("Assets/appconstants.asset");
        }

        public static AppConfig LoadAppConfig()
        {
            return LoadAppConfig(SettingsManager.gameConfigPath);
        }

        public static AppConfig LoadAppConfig(string path)
        {
            return LoadJsonObj<AppConfig>(path);
        }

        public static void SaveAppConfig(AppConfig config)
        {
            SaveAppConfig(config, SettingsManager.gameConfigPath);
        }

        public static void SaveAppConfig(AppConfig config, string path)
        {
            SaveJsonObj(config, path);
        }

        public static PlatformConfig LoadPlatformConfig()
        {
            return LoadPlatformConfig(SettingsManager.platformConfigFileName);
        }

        public static PlatformConfig LoadPlatformConfig(string path)
        {
            return LoadJsonObj<PlatformConfig>(path);
        }

        public static void SavePlatformConfig(PlatformConfig config)
        {
            SaveJsonObj(config, SettingsManager.platformConfigFileName);
        }

        public static void SavePlatformConfig(PlatformConfig config, string path)
        {
            SaveJsonObj(config, path);
        }

        private static void SaveJsonObj<T>(T obj, string path) where T : class
        {
            var json = JsonUtility.ToJson(obj, true);
            File.WriteAllText(path, json, Util.Utf8WithoutBOM());
        }

        private static T LoadJsonObj<T>(string path) where T : class
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var content = File.ReadAllText(path);
            var obj = JsonUtility.FromJson<T>(content);
            return obj;
        }

    }
}
