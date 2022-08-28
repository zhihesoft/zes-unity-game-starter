using System;
using UnityEditor;

namespace Zes.Builders
{
    public class BuildApp : BuildTask
    {
        public BuildApp(AppConstants constants, BuildTarget target) : base(constants, target) { }

        public override string name => "App";

        protected override void AfterBuild()
        {
        }

        protected override bool BeforeBuild()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("ERROR", "Cannot build apk in playing or compiling", "OK");
                return false;
            }

            return true;
        }

        protected override bool OnBuild()
        {
            Util.EnsureDir(constants.outputDir);
            string extension = "";
            if (EditorHelper.usingAAB(target))
            {
                extension = ".aab";
            }
            else if (target == BuildTarget.Android)
            {
                extension = ".apk";
            }
            string outputPath = string.Format("{0}/{1}-{2}-{3}{4}",
                constants.outputDir,
                constants.shortName,
                EditorHelper.CurrentVersion(),
                DateTime.Now.ToString("yyMMddHHmm"),
                extension
                );

            string[] scenes = EditorHelper.GetBuildScenes();

            BuildPlayerOptions opts = new BuildPlayerOptions()
            {
                locationPathName = outputPath,
                scenes = scenes,
                target = BuildTarget.Android

            };

            EditorUserBuildSettings.buildAppBundle = false;

            var report = BuildPipeline.BuildPlayer(opts);

            //if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Failed)
            //{
            //    logger.error($"build {buildConfig.name} apk failed: {report.summary.totalErrors} errors");
            //}
            //if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            //{
            //    logger.info($"build {buildConfig.name} succ !!");
            //}
            //if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Cancelled)
            //{
            //    logger.warn($"build {buildConfig.name} cancel");
            //}
            //if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Unknown)
            //{
            //    logger.error($"build {buildConfig.name} apk failed for unknown reason");
            //}

            return true;
        }
    }
}
