using System;
using UnityEditor;

namespace Zes.Builders
{
    public class BuildApp : BuildTask
    {

        public static string outputDir
        {
            get
            {
                return EditorPrefs.GetString("appOutputPathKey", "");
            }
            set
            {
                EditorPrefs.SetString("appOutputPathKey", value);
            }
        }


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
            var appConfig = EditorHelper.LoadAppConfig();
            Util.EnsureDir(outputDir);
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
                outputDir,
                appConfig.appShortName,
                EditorHelper.CurrentVersion(),
                DateTime.Now.ToString("yyMMddHHmm"),
                extension
                );

            string[] scenes = EditorHelper.GetBuildScenes();

            BuildPlayerOptions opts = new BuildPlayerOptions()
            {
                locationPathName = outputPath,
                scenes = scenes,
                target = target,
            };

            EditorUserBuildSettings.buildAppBundle = false;
#if USING_AAB
            opts.targetGroup = BuildTargetGroup.Android;
            Google.Android.AppBundle.Editor.Internal.AppBundlePublisher.Build(opts, null, true);
#else
            var report = BuildPipeline.BuildPlayer(opts);

            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Failed)
            {
                logger.Error($"build failed: {report.summary.totalErrors} errors");
            }
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                logger.Info($"build succ !!");
            }
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Cancelled)
            {
                logger.Warn($"build cancel");
            }
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Unknown)
            {
                logger.Error($"build failed for unknown reason");
            }
#endif
            return true;
        }
    }
}
