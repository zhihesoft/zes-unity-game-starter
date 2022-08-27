﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Zes
{
    public class BundleBuilder
    {
        static Logger logger = Logger.GetLogger<BundleBuilder>();

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
            appConfig = EditorHelper.LoadAppConfig();
            constants = EditorHelper.LoadAppConstants();
            this.target = target;
#if USING_AAB
            copyToStreaming = false;
#else
            copyToStreaming = true;
#endif
            targetName = target.ToString();
            string dir = Util.EnsureDir(constants.bundleOutputPath).FullName;
            bundlesDir = Util.EnsureDir(Path.Combine(dir, targetName)).FullName;
            streamingDir = Util.EnsureDir(Application.streamingAssetsPath).FullName;
            buildNo = BuildNo.Get(); // GetBuildNo();
        }

        readonly AppConfig appConfig;
        readonly AppConstants constants;
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
            CopyFilesToStreaming(patchInfo);

            BuildNo.Inc();
            logger.Info("asset bundles build finished. " + "(" + buildNo + ")");
            return versionInfo.version;
        }


        VersionInfo CreateVersionInfo()
        {
            VersionInfo vi = new VersionInfo();
            vi.url = appConfig.patchServer + targetName.ToLower();
            vi.version = Application.version + "." + buildNo;
            vi.minVersion = appConfig.minVersion;
            vi.Save(Path.Combine(bundlesDir, constants.versionInfoFile));
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

            string path = Path.Combine(bundlesDir, constants.patchDataPath);
            pi.Save(path);
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
            File.Copy(Path.Combine(bundlesDir, constants.versionInfoFile), Path.Combine(streamingDir, constants.versionInfoFile), true);
            File.Copy(Path.Combine(bundlesDir, constants.patchInfoFile), Path.Combine(streamingDir, constants.patchInfoFile), true);
        }

        void ClearDirs()
        {
            Util.ClearDir(bundlesDir);
            Util.ClearDir(streamingDir);
        }
    }
}
