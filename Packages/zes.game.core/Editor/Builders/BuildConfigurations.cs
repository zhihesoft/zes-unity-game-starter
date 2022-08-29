using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace Zes.Builders
{
    public class BuildConfigurations : BuildTask
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


        public override string name => "Configurations";

        protected override void AfterBuild() { }

        protected override bool BeforeBuild()
        {
            if (!Directory.Exists(excelsPath))
            {
                logger.Error($"{excelsPath} not exists !!");
                return false;
            }

            return true;
        }

        protected override bool OnBuild()
        {
            DirectoryInfo excelDir = new DirectoryInfo(excelsPath);
            DirectoryInfo workDir = excelDir.Parent;
            DirectoryInfo outDir = new DirectoryInfo(Path.Combine(workDir.FullName, "output"));
            DirectoryInfo outClientDir = new DirectoryInfo(Path.Combine(workDir.FullName, "output", "client"));
            DirectoryInfo outLanguageDir = new DirectoryInfo(Path.Combine(workDir.FullName, "output", "language"));
            DirectoryInfo targetDir = new DirectoryInfo(platformConfig.configurationBundlePath);
            Util.EnsureDir(targetDir.FullName);
            Util.EnsureDir(outDir.FullName);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = workDir.FullName;
            startInfo.FileName = "zes-excel-exporter";
            startInfo.Arguments = string.Join(' ',
                "-i", excelDir.FullName,
                "-o", outDir.FullName,
                "--lid", platformConfig.languageStartId,
                "-l", platformConfig.languageConfigName);
            logger.Info($"run {startInfo.FileName} {startInfo.Arguments}");
            var proc = Process.Start(startInfo);
            proc.WaitForExit();

            if (proc.ExitCode != 0)
            {
                logger.Error("excel export failed with code " + proc.ExitCode);
                return false;
            }

            Util.CopyDir(outClientDir, targetDir);
            Util.CopyDir(outLanguageDir, targetDir);

            return true;
        }
    }
}
