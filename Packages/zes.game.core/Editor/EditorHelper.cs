using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public static string[] GetBuildScenes()
        {
            string[] scenes = EditorBuildSettings.scenes
                         .Where(scene => scene.enabled)
                         .Select(scene => scene.path)
                         .Select(s =>
                         {
                             return s;
                         })
                         .ToArray();
            return scenes;
        }

        public static bool usingAAB(BuildTarget target)
        {
            if (target != BuildTarget.Android)
            {
                return false;
            }

#if USING_AAB
            return true;
#else
            return false;
#endif
        }

        public static string CurrentVersion()
        {
            return $"{Application.version}.{BuildNo.Get()}";
        }

        public static int Shell(string filename, string arguments, string workingDir = null)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            if (!string.IsNullOrEmpty(workingDir))
            {
                startInfo.WorkingDirectory = workingDir;
            }
            startInfo.FileName = filename;
            startInfo.Arguments = arguments;
            var proc = Process.Start(startInfo);
            proc.WaitForExit();
            return proc.ExitCode;
        }

        /// <summary>
        /// clear any file in project if it present in template dir
        /// </summary>
        /// <param name="templateDir"></param>
        /// <returns></returns>
        public static void ClearTemplateFiles(string templateDir)
        {
            ClearTemplateFiles(templateDir, null);
        }

        private static void ClearTemplateFiles(string templateDir, string childDir)
        {
            childDir = childDir ?? ".";
            string fromDir = Path.Combine(templateDir, childDir);
            string toDir = Path.Combine(".", childDir);
            var dir = new DirectoryInfo(fromDir);
            dir.GetFiles().ToList().ForEach(file =>
            {
                string toFile = Path.Combine(toDir, file.Name);
                if (File.Exists(toFile))
                {
                    File.Delete(toFile);
                }
            });
            dir.GetDirectories().ToList().ForEach(dir =>
            {
                ClearTemplateFiles(templateDir, Path.Combine(childDir, dir.Name));
            });
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
