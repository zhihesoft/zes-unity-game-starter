using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace ZEditor
{
    public class OpenTSProject
    {
        [MenuItem("ZES/Open TS Project")]
        public static void Open()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "code";
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "ts-source");
            info.Arguments = dir;
            Process.Start(info);
        }

        [MenuItem("ZES/Open C# Project")]
        public static void OpenCSharp()
        {
            EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
        }

    }
}
