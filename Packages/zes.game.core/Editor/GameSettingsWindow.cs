// using Liv;
using UnityEditor;
using UnityEngine;
using Zes.Settings;

namespace Zes
{
    public class GameSettingsWindow : EditorWindow
    {

        [MenuItem("ZES/Settings", priority = 1)]
        public static void ShowGameSettingsWindow()
        {
            var window = GetWindow<GameSettingsWindow>("游戏设置");
            window.Show();
            window.minSize = new Vector2(800, 600);
            window.Initialize();
        }

        private SettingsManager<AppConfig> manager;

        void Initialize()
        {
            if (manager == null)
            {
                manager = new SettingsManager<AppConfig>();
                manager.Initialize();
            }
        }

        private void OnGUI()
        {
            if (manager == null)
            {
                Initialize();
            }
            manager?.OnGUI();
        }

        //        void Initialize()
        //        {
        //            allBaseConfig = new DirectoryInfo(Path.Combine("GameSettings", "configs")).GetDirectories().Select(d => d.Name).ToArray();
        //            allPlatformConfig = new DirectoryInfo(Path.Combine("GameSettings", "platforms")).GetDirectories().Select(d => d.Name).ToArray();
        //            ReloadConfigs();
        //        }

        //        Config config;
        //        PlatformConfig platformConfig;

        //        int currentIndexOfBaseConfig;
        //        int currentIndexOfPlatformBuildConfig;
        //        string[] allBaseConfig;
        //        string[] allPlatformConfig;

        //        private void ReloadConfigs()
        //        {
        //            config = Config.load();
        //            platformConfig = PlatformConfig.load();

        //            currentIndexOfBaseConfig = allBaseConfig.ToList().IndexOf(config.name);
        //            currentIndexOfPlatformBuildConfig = allPlatformConfig.ToList().IndexOf(platformConfig.name);
        //        }

        //        private void OnGUI()
        //        {
        //            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        //            {
        //                currentIndexOfBaseConfig = EditorGUILayout.Popup("基础配置", currentIndexOfBaseConfig, allBaseConfig);
        //                currentIndexOfPlatformBuildConfig = EditorGUILayout.Popup("平台配置", currentIndexOfPlatformBuildConfig, allPlatformConfig);
        //                using (new EditorGUILayout.HorizontalScope())
        //                {
        //                    GUILayout.FlexibleSpace();
        //                    if (GUILayout.Button("应用当前配置"))
        //                    {
        //                        ApplyConfigs(allBaseConfig[currentIndexOfBaseConfig], allPlatformConfig[currentIndexOfPlatformBuildConfig]);
        //                    }
        //                }
        //                EditorGUILayout.Space();
        //            }

        //            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        //            {
        //                EditorGUILayout.LabelField("基础配置");
        //                EditorGUILayout.TextField("配置名称", config.name);
        //                EditorGUILayout.TextField("补丁服务器地址", config.patchServer);
        //                EditorGUILayout.TextField("登录服务器地址", config.loginServer);
        //                EditorGUILayout.Toggle("是否检查更新", config.checkUpdate);
        //                EditorGUILayout.Toggle("允许游客登录", config.allowGuest);
        //                EditorGUILayout.TextField("语言", config.language.ToString());
        //            }

        //            EditorGUILayout.Space();

        //            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        //            {
        //                EditorGUILayout.LabelField("平台配置");
        //                EditorGUILayout.TextField("配置名称", platformConfig.name);
        //                EditorGUILayout.TextField("当前版本", Application.version + "." + BuildNo.Get());
        //                EditorGUILayout.TextField("BuildNo", "" + BuildNo.Get());
        //                EditorGUILayout.TextField("包名", platformConfig.applicationId);
        //                EditorGUILayout.TextField("Android SDK", platformConfig.targetSdkVersion.ToString());
        //                EditorGUILayout.Toggle("是否使用谷歌AAB",
        //#if USING_AAB
        //                        true
        //#else
        //                        false
        //#endif
        //                        );

        //                EditorGUILayout.Toggle("是否使用TalkingData",
        //#if USING_TDGA
        //                        true
        //#else
        //                        false
        //#endif
        //                        );

        //                EditorGUILayout.LabelField("描述", platformConfig.description);
        //            }

        //            EditorGUILayout.Space();

        //            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        //            {
        //                EditorGUILayout.LabelField("配置资源导入");
        //                BuildTextAssets.configPath = EditorGUILayout.TextField("TABLE路径", BuildTextAssets.configPath);
        //                using (new EditorGUILayout.HorizontalScope())
        //                {
        //                    GUILayout.FlexibleSpace();

        //                    if (GUILayout.Button("开始导入"))
        //                    {
        //                        BuildTextAssets.Build();
        //                        EditorUtility.DisplayDialog("导入", "导入完成", "OK");
        //                    }
        //                }
        //                EditorGUILayout.Space();
        //            }

        //            EditorGUILayout.Space();

        //            bool startBuildAPK = false;
        //            bool startBuildBundles = false;
        //            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        //            {
        //                EditorGUILayout.LabelField("打包");

        //                using (new EditorGUILayout.HorizontalScope())
        //                {
        //                    GUILayout.FlexibleSpace();
        //                    startBuildBundles = GUILayout.Button("打包数据");
        //                    startBuildAPK = GUILayout.Button("生成APP");
        //                }
        //                EditorGUILayout.Space();
        //            }

        //            if (startBuildAPK || startBuildBundles)
        //            {
        //                BuildBundlesAndApk(startBuildAPK);
        //                startBuildAPK = false;
        //                EditorUtility.DisplayDialog("完成", "编译完成", "OK");
        //                OpenDir("out");
        //            }
        //        }

        //        public static void BuildOnlyBundles()
        //        {
        //            BuildBundlesAndApk(false);
        //        }

        //        // [MenuItem("SKIN/编译当前配置")]
        //        static void BuildBundlesAndApk(bool includeApk)
        //        {
        //            PlatformConfig buildConfig = PlatformConfig.load();
        //            Config config = Config.load();

        //            int bundleCode = BuildNo.Get();
        //            string apkname = string.Join("_", "skin", config.name, buildConfig.name, Application.version + "." + bundleCode);
        //            BuildTextAssets.Build();
        //            string version = BundleBuilder.BuildBundleOfTarget();
        //            Debug.Log($"数据打包完成，版本为：{version}");

        //            BuildPatchZip(apkname);
        //            if (includeApk)
        //            {
        //                BuildApk.Build(apkname, bundleCode, buildConfig);
        //            }
        //            Debug.Log($"打包完成");
        //        }

        //        static void BuildPatchZip(string name)
        //        {
        //            var localDir = new DirectoryInfo(Path.Combine("AssetBundles", EditorUserBuildSettings.activeBuildTarget.ToString()));
        //            var outDir = new DirectoryInfo("out");
        //            if (!outDir.Exists)
        //            {
        //                outDir.Create();
        //            }

        //            FileInfo zipFile = new FileInfo(Path.Combine(outDir.FullName, name + ".zip"));
        //            using (ZipFile zip = ZipFile.Create(zipFile.FullName))
        //            {
        //                zip.BeginUpdate();
        //                localDir.GetFiles("*.*").ToList().ForEach(file => zip.Add(file.FullName, file.Name));
        //                zip.CommitUpdate();
        //            }
        //        }

        //        void ApplyConfigs(string configName, string platformConfigName)
        //        {
        //            var targetSettingsDir = new DirectoryInfo(Path.Combine("Assets", "Settings"));

        //            var targetPluginsDir = new DirectoryInfo(Path.Combine(targetSettingsDir.FullName, "Plugins"));
        //            if (!targetPluginsDir.Exists)
        //            {
        //                targetPluginsDir.Create();
        //            }
        //            var targetResourcesDir = new DirectoryInfo(Path.Combine(targetSettingsDir.FullName, "Resources"));
        //            if (!targetResourcesDir.Exists)
        //            {
        //                targetResourcesDir.Create();
        //            }

        //            var sourcePlatformDir = new DirectoryInfo(Path.Combine("GameSettings", "platforms", platformConfigName));
        //            var sourceResourcesDir = new DirectoryInfo(Path.Combine("GameSettings", "configs", configName));
        //            var sourcePluginsDir = new DirectoryInfo(Path.Combine(sourcePlatformDir.FullName, "Plugins"));
        //            // copy resource files
        //            ClearDir(targetResourcesDir);
        //            CopyDir(sourceResourcesDir, targetResourcesDir);

        //            // copy plugin files
        //            ClearDir(targetPluginsDir);
        //            if (sourcePluginsDir.Exists)
        //            {
        //                CopyDir(sourcePluginsDir, targetPluginsDir);
        //            }


        //            // copy buildconfig to project root
        //            var platformFiles = new string[] { "platform.json", "csc.rsp" };
        //            foreach (var platformFile in platformFiles)
        //            {
        //                var fi = new FileInfo(Path.Combine(sourcePlatformDir.FullName, platformFile));
        //                fi.CopyTo(platformFile, true);
        //            }

        //            // generate buildconfig.json
        //            PlatformConfig p = PlatformConfig.load();
        //            BuildConfig buildconfig = new BuildConfig();
        //            buildconfig.platform = p.name;
        //            buildconfig.applicationId = p.applicationId;

        //            var utf8 = new UTF8Encoding(false);
        //            using (StreamWriter fs = new StreamWriter(Path.Combine(targetResourcesDir.FullName, "buildconfig.json"), false, utf8))
        //            {
        //                string json = JsonUtility.ToJson(buildconfig);
        //                fs.Write(json);
        //            }

        //            AssetDatabase.ImportAsset("Assets/Settings");
        //            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        //            ReloadConfigs();
        //        }

        //        void OpenDir(string dir)
        //        {
        //            try
        //            {
        //                System.Diagnostics.Process.Start(dir);
        //            }
        //            catch (System.Exception ex)
        //            {
        //                //The system cannot find the file specified...
        //                Debug.LogError(ex.Message);
        //            }
        //        }

        //        void CopyDir(DirectoryInfo source, DirectoryInfo dest)
        //        {
        //            EnsureDir(dest);
        //            ClearDir(dest);

        //            if (!source.Exists)
        //            {
        //                return;
        //            }

        //            var files = source.GetFiles();
        //            foreach (FileInfo s in files)
        //            {
        //                var destFile = Path.Combine(dest.FullName, s.Name);
        //                File.Copy(s.FullName, destFile, true);
        //            }

        //            var dirs = source.GetDirectories();
        //            foreach (var dir in dirs)
        //            {
        //                var destDir = Path.Combine(dest.FullName, dir.Name);
        //                DirectoryInfo di = new DirectoryInfo(destDir);
        //                CopyDir(dir, di);
        //            }
        //        }

        //        void EnsureDir(DirectoryInfo dir)
        //        {
        //            if (!dir.Parent.Exists)
        //            {
        //                EnsureDir(dir.Parent);
        //            }

        //            if (!dir.Exists)
        //            {
        //                dir.Create();
        //            }
        //        }

        //        void ClearDir(DirectoryInfo dir)
        //        {
        //            dir.GetFiles().ToList().ForEach(f => f.Delete());
        //            dir.GetDirectories().ToList().ForEach(d => d.Delete(true));
        //        }
    }
}
