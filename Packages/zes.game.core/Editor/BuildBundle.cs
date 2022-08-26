using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Zes
{
    public class BundleBuilder
    {

        public static string BuildBundleOfTarget()
        {
            return BuildBundleOfTarget(EditorUserBuildSettings.activeBuildTarget);
        }

        public static string BuildBundleOfTarget(BuildTarget target)
        {
            BundleBuilder proc = new BundleBuilder(target);
            return proc.Build();
        }

        BundleBuilder(BuildTarget target)
        {
            appConfig = AppConfig.Load();
            appProp = AssetDatabase.LoadAssetAtPath<AppProp>("app.asset"); // AppProp.Load();
            Debug.Assert(appProp != null, "app.asset should existed");

            this.target = target;
#if USING_AAB
            copyToStreaming = false;
#else
            copyToStreaming = true;
#endif
            targetName = target.ToString();

            string dir = Util.EnsureDir("AssetBundles").FullName;
            bundlesDir = Util.EnsureDir(Path.Combine(dir, targetName)).FullName;
            streamingDir = Util.EnsureDir(Application.streamingAssetsPath).FullName;

            buildNo = BuildNo.Get(); // GetBuildNo();
        }

        readonly AppConfig appConfig;
        readonly AppProp appProp;
        readonly BuildTarget target;
        readonly int buildNo;
        readonly bool copyToStreaming;
        readonly string targetName;
        readonly string streamingDir;
        readonly string bundlesDir;
        Dictionary<string, string> allFiles;

        public string Build()
        {
            Caching.ClearCache();
            ClearDirs();
            var manifest = BuildPipeline.BuildAssetBundles(bundlesDir, BuildAssetBundleOptions.None, target);

            var files = manifest.GetAllAssetBundles().ToList();
            allFiles = files.ToDictionary(file => file, file => manifest.GetAssetBundleHash(file).ToString());

            VersionInfo versionInfo = CreateVersionInfo();
            PatchInfo patchInfo = CreatePatchInfo();
            CopyFilesToStreaming(patchInfo, copyToStreaming);

            BuildNo.Inc();
            Debug.Log("asset bundles build finished. " + "(" + buildNo + ")");
            return versionInfo.version;
        }


        VersionInfo CreateVersionInfo()
        {
            VersionInfo vi = new VersionInfo();
            vi.url = appConfig.patchServer + targetName.ToLower();
            vi.version = Application.version + "." + buildNo;
            vi.minVersion = appConfig.minVersion;
            vi.Save(Path.Combine(bundlesDir, appProp.versionInfoFile));
            return vi;
        }

        PatchInfo CreatePatchInfo()
        {
            PatchInfo pi = new PatchInfo();
            pi.url = appConfig.patchServer + targetName.ToLower();
            pi.version = Application.version + "." + buildNo;
            pi.files = allFiles
                .Select(file => new PatchFileInfo()
                {
                    path = file.Key,
                    md5 = file.Value,
                    size = PatchInfoFileSize(file.Key)
                })
                .ToArray();

            string path = Path.Combine(bundlesDir, appProp.patchDataPath);
            pi.Save(path);
            return pi;
        }

        int PatchInfoFileSize(string path)
        {
            FileInfo fi = new FileInfo(Path.Combine(bundlesDir, path));
            return (int)fi.Length;
        }

        void CopyFilesToStreaming(PatchInfo pi, bool copyAll)
        {
            // clear stream dir
            DirectoryInfo di = new DirectoryInfo(streamingDir);
            di.GetFiles().ToList().ForEach(file => file.Delete());

            if (copyAll)
            {
                foreach (var file in pi.files)
                {
                    string fullpath = Path.Combine(bundlesDir, file.path);
                    if (!File.Exists(fullpath))
                    {
                        Debug.LogError("cannot find file: " + fullpath);
                        throw new System.Exception("file " + fullpath + " not found");
                    }
                    File.Copy(fullpath, Path.Combine(streamingDir, file.path));
                }
            }
            File.Copy(Path.Combine(bundlesDir, appProp.versionInfoFile), Path.Combine(streamingDir, appProp.versionInfoFile), true);
            File.Copy(Path.Combine(bundlesDir, appProp.patchInfoFile), Path.Combine(streamingDir, appProp.patchInfoFile), true);
        }

        void ClearDirs()
        {
            Util.ClearDir(bundlesDir);
            Util.ClearDir(streamingDir);
        }
    }
}
