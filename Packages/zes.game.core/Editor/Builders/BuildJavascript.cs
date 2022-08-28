using System.IO;
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
            var exitCode = EditorHelper.Shell("gulp", "build", constants.javascriptProjectPath);
            if (exitCode != 0)
            {
                logger.Error("build ts source failed with code " + exitCode);
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
