using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace ZEditor
{
    public static class BuildApk
    {

        public static void Build(string path, int bundleCode, PlatformConfig buildConfig)
        {
            if (EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("错误", "运行中无法构建", "OK");
                return;
            }

            SetPlayerSettings(bundleCode, buildConfig);

            string apkName = $"out/{path}.apk";

            Debug.Log($"output apk name is {apkName}");

            string[] scenes = EditorBuildSettings.scenes
                         .Where(scene => scene.enabled)
                         .Select(scene => scene.path)
                         .Select(s =>
                         {
                             Debug.Log($"add scene {s} to build list");
                             return s;
                         })
                         .ToArray();

            BuildPlayerOptions opts = new BuildPlayerOptions()
            {
                locationPathName = apkName,
                scenes = scenes,
                target = BuildTarget.Android

            };

#if USING_AAB
        // Check that we're ready to build before displaying the file save dialog.
        Google.Android.AppBundle.Editor.Internal.AppBundlePublisher.Build($"out/{path}.aab");
#else
            EditorUserBuildSettings.buildAppBundle = false;

            var report = BuildPipeline.BuildPlayer(opts);

            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Failed)
            {
                Debug.LogError($"build {buildConfig.name} apk failed: {report.summary.totalErrors} errors");
            }
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"build {buildConfig.name} succ !!");
            }
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Cancelled)
            {
                Debug.LogWarning($"build {buildConfig.name} cancel");
            }
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Unknown)
            {
                Debug.LogError($"build {buildConfig.name} apk failed for unknown reason");
            }
#endif
            Debug.Log($"Build Done~");
        }

        static void SetPlayerSettings(int bundleCode, PlatformConfig buildConfig)
        {
            PlayerSettings.companyName = buildConfig.company;
            PlayerSettings.productName = buildConfig.productName;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, buildConfig.applicationId);
            PlayerSettings.Android.keystoreName = buildConfig.keystore;
            PlayerSettings.Android.keystorePass = buildConfig.keystorePassword;
            PlayerSettings.Android.keyaliasName = buildConfig.keyAlias;
            PlayerSettings.Android.keyaliasPass = buildConfig.keyAliasPassword;
            PlayerSettings.Android.targetSdkVersion = buildConfig.targetSdkVersion;
            PlayerSettings.Android.bundleVersionCode = bundleCode;
        }


        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            Debug.Log("OnPostprocessBuild:" + pathToBuiltProject);
#if USING_FIREBASE
#endif
        }
    }
}
