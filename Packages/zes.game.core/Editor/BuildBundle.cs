namespace ZEditor
{
    public class BundleBuilder
    {

        //        public static string BuildBundleOfTarget()
        //        {
        //            return BuildBundleOfTarget(EditorUserBuildSettings.activeBuildTarget);
        //        }

        //        public static string BuildBundleOfTarget(BuildTarget target)
        //        {
        //            BundleBuilder proc = new BundleBuilder(target);
        //            return proc.Build();
        //        }

        //        BundleBuilder(BuildTarget target)
        //        {
        //            config = Config.load();
        //            buildConfig = PlatformConfig.load();

        //            this.target = target;
        //#if USING_AAB
        //            copyToStreaming = false;
        //#else
        //            copyToStreaming = true;
        //#endif
        //            targetName = target.ToString();

        //            string dir = EnsureDir("AssetBundles");
        //            bundlesDir = EnsureDir(Path.Combine(dir, targetName));
        //            streamingDir = EnsureDir(Application.streamingAssetsPath);

        //            buildNo = BuildNo.Get(); // GetBuildNo();
        //        }

        //        readonly Config config;
        //        readonly PlatformConfig buildConfig;
        //        readonly BuildTarget target;
        //        readonly int buildNo;
        //        private readonly bool copyToStreaming;
        //        readonly string targetName;
        //        readonly string streamingDir;
        //        readonly string bundlesDir;
        //        Dictionary<string, string> allFiles;

        //        public string Build()
        //        {
        //            Caching.ClearCache();
        //            ClearDirs();
        //            var manifest = BuildPipeline.BuildAssetBundles(bundlesDir, BuildAssetBundleOptions.None, target);

        //            var files = manifest.GetAllAssetBundles().ToList();
        //            allFiles = files.ToDictionary(file => file, file => manifest.GetAssetBundleHash(file).ToString());

        //            VersionInfo versionInfo = CreateVersionInfo();
        //            PatchInfo patchInfo = CreatePatchInfo();
        //            CopyFilesToStreaming(patchInfo, copyToStreaming);

        //            BuildNo.Inc();
        //            Debug.Log("asset bundles build finished. " + "(" + buildNo + ")");
        //            return versionInfo.version;
        //        }


        //        VersionInfo CreateVersionInfo()
        //        {
        //            VersionInfo vi = new VersionInfo();
        //            vi.url = config.patchServer + targetName.ToLower();
        //            vi.version = Application.version + "." + buildNo;
        //            vi.minVersion = config.minVersion;
        //            vi.Save(Path.Combine(bundlesDir, PatchConstants.versionInfoFile));
        //            return vi;
        //        }

        //        PatchInfo CreatePatchInfo()
        //        {
        //            PatchInfo pi = new PatchInfo();
        //            pi.url = config.patchServer + targetName.ToLower();
        //            pi.version = Application.version + "." + buildNo;
        //            pi.files = allFiles
        //                .Select(file => new PatchFileInfo()
        //                {
        //                    path = file.Key,
        //                    md5 = file.Value,
        //                    size = PatchInfoFileSize(file.Key)
        //                })
        //                .ToArray();

        //            string path = Path.Combine(bundlesDir, PatchConstants.patchInfoFile);
        //            pi.Save(path);
        //            return pi;
        //        }

        //        int PatchInfoFileSize(string path)
        //        {
        //            FileInfo fi = new FileInfo(Path.Combine(bundlesDir, path));
        //            return (int)fi.Length;
        //        }

        //        void CopyFilesToStreaming(PatchInfo pi, bool copyAll)
        //        {
        //            // clear stream dir
        //            DirectoryInfo di = new DirectoryInfo(streamingDir);
        //            di.GetFiles().ToList().ForEach(file => file.Delete());

        //            if (copyAll)
        //            {
        //                foreach (var file in pi.files)
        //                {
        //                    string fullpath = Path.Combine(bundlesDir, file.path);
        //                    if (!File.Exists(fullpath))
        //                    {
        //                        Debug.LogError("cannot find file: " + fullpath);
        //                        throw new System.Exception("file " + fullpath + " not found");
        //                    }
        //                    File.Copy(fullpath, Path.Combine(streamingDir, file.path));
        //                }
        //            }
        //            File.Copy(Path.Combine(bundlesDir, PatchConstants.versionInfoFile), Path.Combine(streamingDir, PatchConstants.versionInfoFile), true);
        //            File.Copy(Path.Combine(bundlesDir, PatchConstants.patchInfoFile), Path.Combine(streamingDir, PatchConstants.patchInfoFile), true);
        //        }

        //        void ClearDirs()
        //        {
        //            ClearDir(bundlesDir);
        //            ClearDir(streamingDir);
        //        }

        //        void ClearDir(string dir)
        //        {
        //            DirectoryInfo di = new DirectoryInfo(dir);
        //            di.GetDirectories().ToList().ForEach(child => child.Delete(true));
        //            di.GetFiles().ToList().ForEach(file => file.Delete());
        //        }

        //        string EnsureDir(string dir)
        //        {
        //            if (!Directory.Exists(dir))
        //                Directory.CreateDirectory(dir);
        //            return dir;
        //        }

    }
}
