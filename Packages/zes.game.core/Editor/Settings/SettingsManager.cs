using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Zes.Settings
{
    /// <summary>
    /// settings manager, game can override this class
    /// T is config type
    /// </summary>
    public class SettingsManager<T> where T : AppConfig, new()
    {
        public const string settingsSourceDir = "GameSettings";
        public const string settingsTargetDir = "Assets/GameSettings";
        public const string gameConfigPath = settingsTargetDir + "/Resources/boot.json";
        public const string platformConfigFileName = "platform.json";
        public const string gameConfigFileName = "platform.json";
        public const string templatePathName = "templates";
        public const string configSourcePathName = "configs";
        public const string defaultConfigName = "dev";

        private Logger logger = Logger.GetLogger<GameSettingsWindow>();

        protected Dictionary<string, SettingPanel> panels = new Dictionary<string, SettingPanel>();
        protected SettingPanel currentPanel;
        protected T gameConfig;
        protected PlatformConfig platformConfig;
        private string[] allConfigs;
        private int currentIndexOfConfig = -1;
        private bool showCreatePanel = false;
        private string newPlatformName = "";

        public virtual void Initialize()
        {
            InitializePanels();
            currentPanel = panels.Values.First();
            currentPanel.config = gameConfig;
            currentPanel.OnShow();

            allConfigs = new DirectoryInfo(settingsSourceDir).GetDirectories()
                .SelectMany(p => new DirectoryInfo(Path.Combine(p.FullName, configSourcePathName))
                .GetDirectories()
                .Where(d => d.Name != templatePathName)
                .Select(d => $"{p.Name}/{d.Name}")).ToArray();

            LoadGameConfig();
            LoadPlatformConfig();

            if (IsConfigValid())
            {
                var currentconfigname = $"{platformConfig.name}/{gameConfig.name}";
                currentIndexOfConfig = allConfigs.ToList().IndexOf(currentconfigname);
            }
        }

        protected virtual void InitializePanels()
        {
            panels.Clear();
            AddPanel(new CommonPanel());
            AddPanel(new PatchPanel());
            AddPanel(new PlatformPanel());
        }


        public void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.LabelField("Project is compiling !!",
                    EditorStyles.centeredGreyMiniLabel,
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                return;
            }

            ConfigSelector();
            DrawCreatePanel();

            if (!IsConfigValid())
            {
                EditorGUILayout.LabelField("Config invalid !!",
                    EditorStyles.centeredGreyMiniLabel,
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                return;
            }

            bool refresAsset = false;
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                SectionSelector();
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    if (currentPanel != null)
                    {
                        EditorGUILayout.LabelField(currentPanel.Description);
                        currentPanel.OnGUI();
                        if (currentPanel.dirty)
                        {
                            SaveGameConfig();
                            SavePlatformConfig();
                            currentPanel.dirty = false;
                            refresAsset = true;
                        }
                    }
                }
            }
            if (refresAsset)
            {
                AssetDatabase.Refresh();
            }
        }

        protected virtual bool LoadGameConfig()
        {
            if (!File.Exists(gameConfigPath))
            {
                return false;
            }
            var content = File.ReadAllText(gameConfigPath);
            gameConfig = JsonUtility.FromJson<T>(content);
            panels.Values.ToList().ForEach(i => i.config = gameConfig);
            return true;
        }

        protected virtual void SaveGameConfig()
        {
            SaveGameConfig(gameConfig, gameConfigPath);
        }

        protected virtual void SaveGameConfig(AppConfig config, string targetPath)
        {
            var json = JsonUtility.ToJson(config, true);
            File.WriteAllText(targetPath, json, Util.Utf8WithoutBOM());
        }

        protected bool LoadPlatformConfig()
        {
            if (!File.Exists(platformConfigFileName))
                return false;
            platformConfig = PlatformConfig.load();
            panels.Values.ToList().ForEach(i => i.platformConfig = platformConfig);
            return true;
        }

        protected void SavePlatformConfig()
        {
            SavePlatformConfig(platformConfig, platformConfigFileName);
        }

        protected void SavePlatformConfig(PlatformConfig config, string targetPath)
        {
            var json = JsonUtility.ToJson(config, true);
            File.WriteAllText(targetPath, json, Util.Utf8WithoutBOM());
        }


        protected bool IsConfigValid()
        {
            return gameConfig != null && platformConfig != null;
        }

        private void AddPanel(SettingPanel panel)
        {
            panels.Add(panel.Name, panel);
        }

        private void DrawCreatePanel()
        {
            if (!showCreatePanel)
            {
                return;
            }

            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    newPlatformName = EditorGUILayout.TextField(newPlatformName).Trim();
                    if (GUILayout.Button("Create new platform", GUILayout.Width(200)))
                    {
                        if (!string.IsNullOrEmpty(newPlatformName))
                        {
                            var newConfig = new PlatformConfig();
                            newConfig.name = newPlatformName;
                            var targetdir = Path.Combine(settingsSourceDir, newConfig.name);
                            Util.EnsureDir(targetdir);
                            var templatedir = Path.Combine(targetdir, templatePathName);

                            // create templates dir
                            Util.EnsureDir(templatedir);
                            SavePlatformConfig(newConfig, Path.Combine(templatedir, platformConfigFileName));

                            // create configs dir
                            var configsdir = Path.Combine(targetdir,
                                configSourcePathName,
                                defaultConfigName,
                                "Assets",
                                "GameSettings",
                                "Resources");
                            Util.EnsureDir(configsdir);

                            // create a default config
                            var gameConfig = new AppConfig();
                            SaveGameConfig(new AppConfig(), Path.Combine(configsdir, gameConfigFileName));
                            EditorUtility.DisplayDialog("DONE", $"platform {newPlatformName} created", "OK");
                            newPlatformName = "";
                            showCreatePanel = false;
                        }
                    }
                }
            }
        }

        private void SectionSelector()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(160), GUILayout.ExpandHeight(true)))
            {
                foreach (var item in panels.Values)
                {
                    Color old = GUI.backgroundColor;
                    GUI.backgroundColor = item == currentPanel ? Color.green : old;
                    if (GUILayout.Button(item.DisplayName, GUILayout.Height(32)))
                    {
                        GUIUtility.keyboardControl = 0;
                        if (currentPanel != item)
                        {
                            currentPanel.OnHide();
                        }
                        currentPanel = item;
                        currentPanel.OnShow();
                    }
                    GUI.backgroundColor = old;

                }
            }
        }

        private void ConfigSelector()
        {
            EditorGUILayout.BeginHorizontal();
            var newIndex = EditorGUILayout.Popup(currentIndexOfConfig, allConfigs);
            if (newIndex != currentIndexOfConfig)
            {
                currentIndexOfConfig = newIndex;
                ApplyConfig();
            }
            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(32)))
            {
                showCreatePanel = !showCreatePanel;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ApplyConfig()
        {
            var configname = allConfigs[currentIndexOfConfig];
            var parts = configname.Split('/');
            var platformname = parts[0].Trim();
            configname = parts[1].Trim();

            Util.ClearDir(settingsTargetDir);
            var platformSourceDir = Path.Combine(settingsSourceDir, platformname, templatePathName);
            logger.Info($"copy platform source from {platformSourceDir}");
            Util.CopyDir(platformSourceDir, ".");
            var configSourceDir = Path.Combine(settingsSourceDir, platformname, configSourcePathName, configname);
            logger.Info($"copy config source from {platformSourceDir}");
            Util.CopyDir(configSourceDir, ".");

            LoadGameConfig();
            LoadPlatformConfig();

            if (gameConfig.name != configname)
            {
                gameConfig.name = configname;
                SaveGameConfig();
            }

            if (platformConfig.name != platformname)
            {
                platformConfig.name = platformname;
                SavePlatformConfig();
            }

            AssetDatabase.Refresh();
        }

    }
}
