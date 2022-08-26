using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Zes.Settings
{
    /// <summary>
    /// settings manager
    /// </summary>
    public class SettingsManager
    {
        private Logger logger = Logger.GetLogger<GameSettingsWindow>();
        public const string settingsSourceDir = "GameSettings";
        public const string settingsTargetDir = "Assets/GameSettings";
        public const string gameConfigFileName = "boot.json";
        public const string gameConfigPath = "Assets/" + gameConfigFileName;
        public const string platformConfigFileName = "platform.json";
        public const string templatePathName = "templates";
        public const string configSourcePathName = "configs";
        public const string defaultConfigName = "dev";

        public AppConfig gameConfig { get; protected set; }
        public PlatformConfig platformConfig { get; protected set; }

        protected Dictionary<string, SettingPanel> panels = new Dictionary<string, SettingPanel>();
        protected SettingPanel currentPanel;
        private string[] allConfigs;
        private int currentIndexOfConfig = -1;
        private bool showCreatePanel = false;
        private string newPlatformName = "";

        public virtual void Initialize()
        {
            InitializePanels();
            currentPanel = panels.Values.First();
            currentPanel.OnShow();

            allConfigs = new DirectoryInfo(settingsSourceDir).GetDirectories().SelectMany(p =>
                new DirectoryInfo(Path.Combine(p.FullName, configSourcePathName))
                    .GetDirectories()
                    .Where(d => d.Name != templatePathName)
                    .Select(d => $"{p.Name}/{d.Name}")
            ).ToArray();

            LoadConfigs();

            if (IsConfigValid())
            {
                var currentconfigname = $"{platformConfig.name}/{gameConfig.name}";
                currentIndexOfConfig = allConfigs.ToList().IndexOf(currentconfigname);
            }
        }

        protected virtual void InitializePanels()
        {
            panels.Clear();
            AddPanel(new GameConfigPanel());
            AddPanel(new PlatformPanel());
            AddPanel(new SpacePanel("space"));
            AddPanel(new BuildPanel());
        }

        private void LoadConfigs()
        {
            gameConfig = EditorHelper.LoadAppConfig();
            platformConfig = EditorHelper.LoadPlatformConfig();
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
                            EditorHelper.SaveAppConfig(gameConfig);
                            EditorHelper.SavePlatformConfig(platformConfig);
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

        protected bool IsConfigValid()
        {
            return gameConfig != null && platformConfig != null;
        }

        private void AddPanel(SettingPanel panel)
        {
            panel.manager = this;
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
                            EditorHelper.SavePlatformConfig(newConfig, Path.Combine(templatedir, platformConfigFileName));

                            // create configs dir
                            var configsdir = Path.Combine(targetdir,
                                configSourcePathName,
                                defaultConfigName,
                                "Assets");
                            Util.EnsureDir(configsdir);

                            // create a default config
                            EditorHelper.SaveAppConfig(new AppConfig(), Path.Combine(configsdir, gameConfigFileName));
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
                    if (item.GetType() == typeof(SpacePanel))
                    {
                        item.OnGUI();
                        continue;
                    }
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
            // delete csc.rsp
            if (File.Exists("csc.rsp"))
            {
                File.Delete("csc.rsp");
            }

            var platformSourceDir = Path.Combine(settingsSourceDir, platformname, templatePathName);
            logger.Info($"copy platform source from {platformSourceDir}");
            Util.CopyDir(platformSourceDir, ".");
            var configSourceDir = Path.Combine(settingsSourceDir, platformname, configSourcePathName, configname);
            logger.Info($"copy config source from {platformSourceDir}");
            Util.CopyDir(configSourceDir, ".");

            LoadConfigs();

            if (gameConfig.name != configname)
            {
                gameConfig.name = configname;
                EditorHelper.SaveAppConfig(gameConfig);
            }

            if (platformConfig.name != platformname)
            {
                platformConfig.name = platformname;
                EditorHelper.SavePlatformConfig(platformConfig);
            }

            AssetDatabase.Refresh();
        }

    }
}
