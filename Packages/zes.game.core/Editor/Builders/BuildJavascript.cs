using System.IO;

namespace Zes.Builders
{
    public class BuildJavascript : BuildTask
    {
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
            var exitCode = EditorHelper.Shell("gulp", "build", platformConfig.javascriptProjectPath);
            if (exitCode != 0)
            {
                logger.Error("build ts source failed with code " + exitCode);
                return false;
            }

            var source = new FileInfo(Path.Combine(platformConfig.javascriptProjectPath, platformConfig.javascriptBuildResult));
            if (!source.Exists)
            {
                logger.Error($"{source.FullName} not existed");
                return false;
            }
            var target = new FileInfo(appConfig.javascriptData);
            source.CopyTo(target.FullName, true);
            return true;
        }
    }
}
