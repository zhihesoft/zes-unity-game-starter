using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Zes.Builders
{
    public class BuildJavascript : BuildTask
    {
        public BuildJavascript(AppConstants constants, BuildTarget target) : base(constants, target) { }

        public override string name => "Javascript";

        protected override void AfterBuild()
        {
        }

        protected override bool BeforeBuild()
        {
            return true;
        }

        protected override bool OnBuild()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = constants.javascriptProjectPath;
            startInfo.FileName = "gulp";
            startInfo.Arguments = "build";
            var proc = Process.Start(startInfo);
            proc.WaitForExit();

            if (proc.ExitCode != 0)
            {
                logger.Error("build ts source failed with code " + proc.ExitCode);
                return false;
            }

            var source = new FileInfo(Path.Combine(constants.javascriptProjectPath, constants.javascriptBuildResult));
            if (!source.Exists)
            {
                logger.Error($"{source.FullName} not existed");
                return false;
            }
            var target = new FileInfo(constants.javascriptEntryRuntime);
            source.CopyTo(target.FullName, true);
            return true;
        }
    }
}
