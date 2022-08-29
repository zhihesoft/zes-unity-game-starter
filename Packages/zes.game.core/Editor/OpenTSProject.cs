using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace Zes
{
    public class OpenTSProject
    {
        [MenuItem("ZES/Open TS Project")]
        public static void Open()
        {
            var config = EditorHelper.LoadPlatformConfig();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "code";
            info.Arguments = config.javascriptProjectPath;
            Process.Start(info);
        }

        [MenuItem("ZES/Open C# Project")]
        public static void OpenCSharp()
        {
            EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
        }

        [MenuItem("ZES/Generate PuerTS codes")]
        public static void GeneratePuerTSCodes()
        {
            DirectoryInfo di = new DirectoryInfo("Assets/Gen");
            if (di.Exists)
            {
                di.Delete(true);
            }

            EditorApplication.ExecuteMenuItem("PuerTS/Generate Code");
        }

    }
}
