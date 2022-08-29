using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Zes.Builders
{
    public class BuildBundle : BuildTask
    {

        private string targetName;
        private string streamingDir;
        private string bundlesDir;
        private Dictionary<string, string> allFiles;
        private int buildNo;

        public override string name => "Bundle";

        protected override void AfterBuild()
        {
            AssetDatabase.Refresh();
        }

        protected override bool BeforeBuild()
        {
            buildNo = BuildNo.Get();
            targetName = target.ToString();
            string dir = Util.EnsureDir(platformConfig.bundleOutputPath).FullName;
            bundlesDir = Util.EnsureDir(Path.Combine(dir, targetName)).FullName;
            streamingDir = Util.EnsureDir(Application.streamingAssetsPath).FullName;

            Util.ClearDir(bundlesDir);
            Util.ClearDir(streamingDir);
            Caching.ClearCache();
            return true;
        }

        protected override bool OnBuild()
        {
            var manifest = BuildPipeline.BuildAssetBundles(bundlesDir, BuildAssetBundleOptions.None, target);
            var files = manifest.GetAllAssetBundles().ToList();
            allFiles = files.ToDictionary(file => file, file => manifest.GetAssetBundleHash(file).ToString());

            VersionInfo versionInfo = CreateVersionInfo();
            PatchInfo patchInfo = CreatePatchInfo();
            CopyFilesToStreaming(patchInfo);

            return true;
        }

        protected override string FinishInfo()
        {
            return $"buildno={buildNo}";
        }

        string url => Util.CombineUri(appConfig.patchServer, targetName.ToLower());


        VersionInfo CreateVersionInfo()
        {
            VersionInfo vi = new VersionInfo();
            vi.url = url;
            vi.version = EditorHelper.CurrentVersion();
            vi.minVersion = appConfig.minVersion;
            vi.Save(Path.Combine(bundlesDir, appConfig.versionInfoFile));
            return vi;
        }

        PatchInfo CreatePatchInfo()
        {
            PatchInfo pi = new PatchInfo();
            pi.url = url;
            pi.version = EditorHelper.CurrentVersion();
            pi.files = allFiles
                .Select(file => new PatchFileInfo()
                {
                    path = file.Key,
                    md5 = file.Value,
                    size = PatchInfoFileSize(file.Key)
                })
                .ToArray();

            pi.Save(Path.Combine(bundlesDir, appConfig.patchInfoFile));
            return pi;
        }

        int PatchInfoFileSize(string path)
        {
            FileInfo fi = new FileInfo(Path.Combine(bundlesDir, path));
            return (int)fi.Length;
        }

        void CopyFilesToStreaming(PatchInfo pi)
        {
            // clear stream dir
            DirectoryInfo di = new DirectoryInfo(streamingDir);
            di.GetFiles().ToList().ForEach(file => file.Delete());

            bool copyToStreaming = EditorHelper.usingAAB(target);

            if (copyToStreaming)
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
            File.Copy(Path.Combine(bundlesDir, appConfig.versionInfoFile), Path.Combine(streamingDir, appConfig.versionInfoFile), true);
            File.Copy(Path.Combine(bundlesDir, appConfig.patchInfoFile), Path.Combine(streamingDir, appConfig.patchInfoFile), true);

            BuildPatchZip(EditorHelper.GetAppOutputName(appConfig, platformConfig));
        }

        void BuildPatchZip(string name)
        {
            var localDir = new DirectoryInfo(Path.Combine("AssetBundles", EditorUserBuildSettings.activeBuildTarget.ToString()));
            Util.EnsureDir(BuildApp.outputDir);
            FileInfo zipFile = new FileInfo(Path.Combine(BuildApp.outputDir, name + ".zip"));
            using (ZipFile zip = ZipFile.Create(zipFile.FullName))
            {
                zip.BeginUpdate();
                localDir.GetFiles("*.*").ToList().ForEach(file => zip.Add(file.FullName, file.Name));
                zip.CommitUpdate();
            }
        }

    }
}
