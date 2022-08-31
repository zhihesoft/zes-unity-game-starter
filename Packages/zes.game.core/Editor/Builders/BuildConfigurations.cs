using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zes.Builders
{
    public class BuildConfigurations : BuildTask
    {
        static string excelsPathKey => $"{Application.productName}-excelFilesPathKey";

        public static string excelsPath
        {
            get
            {
                return EditorPrefs.GetString(excelsPathKey, "");
            }
            set
            {
                EditorPrefs.SetString(excelsPathKey, value);
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
            Util.DirEnsure(targetDir.FullName);
            Util.DirEnsure(outDir.FullName);

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

            Util.DirCopy(outClientDir, targetDir);
            Util.DirCopy(outLanguageDir, targetDir);

            return true;
        }
    }
}
