using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;

namespace ZEditor
{
    public static class BuildTextAssets
    {
        public static string excelsPath
        {
            get
            {
                return EditorPrefs.GetString("excelFilesPathKey", "");
            }
            set
            {
                EditorPrefs.SetString("excelFilesPathKey", value);
            }
        }

        public static void Build()
        {
            if (!BuildTS() || !BuildJsons())
            {
                return;
            }

            // copy js to data dir
            FileInfo js = new FileInfo(Path.Combine("ts-source", "out", "main.mjs"));
            js.CopyTo(Path.Combine("Assets", "Bundles", "data", "main.mjs"), true);

            // copy jsons to data dir
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(excelsPath, "json", "client"));
            var jsons = dir.GetFiles()
                .Where(f => f.Extension == ".json")
                .ToList();
            jsons.ForEach(json =>
            {
                json.CopyTo(Path.Combine("Assets", "Bundles", "data", json.Name.ToLower()), true);
            });
            UnityEngine.Debug.Log("Build XML completed!");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static bool BuildTS()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = "ts-source";
            startInfo.FileName = "gulp";
            startInfo.Arguments = "build";
            var proc = Process.Start(startInfo);
            proc.WaitForExit();

            if (proc.ExitCode != 0)
            {
                UnityEngine.Debug.LogError("build ts source failed with code " + proc.ExitCode);
                return false;
            }
            UnityEngine.Debug.Log("build ts source done");
            return true;
        }

        private static bool BuildJsons()
        {
            if (!Directory.Exists(excelsPath))
            {
                UnityEngine.Debug.LogError($"{excelsPath} not exists !!");
                return false;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = Path.Combine("..", "skin-conv");
            startInfo.FileName = "npm";
            startInfo.Arguments = $"start {Path.Combine(excelsPath, "excel")}";
            var proc = Process.Start(startInfo);
            proc.WaitForExit();

            if (proc.ExitCode != 0)
            {
                UnityEngine.Debug.LogError("excel export failed with code " + proc.ExitCode);
                return false;
            }

            UnityEngine.Debug.Log("excel export done");
            return true;
        }
    }
}
