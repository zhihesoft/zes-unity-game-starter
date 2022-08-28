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
        public const string commonDirName = "common";
        public const string platformDirName = "platforms";
        public const string configDirName = "configs";
        public const string gameConfigFileName = "boot.json";
        public const string gameConfigPath = "Assets/" + gameConfigFileName;
        public const string platformConfigFileName = "platform.json";
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

            string platformdir = Path.Combine(settingsSourceDir, platformDirName);
            string configdir = Path.Combine(settingsSourceDir, configDirName);

            allConfigs = new DirectoryInfo(platformdir).GetDirectories().SelectMany(p =>
                new DirectoryInfo(configdir)
                    .GetDirectories()
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
            // AddPanel(new SpacePanel("space"));
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
                    GUILayout.ExpandWidth(true),
                    GUILayout.ExpandHeight(true));
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
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
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
                            var targetPlatformDir = Path.Combine(settingsSourceDir, platformDirName, newConfig.name);
                            Util.EnsureDir(targetPlatformDir);
                            EditorHelper.SavePlatformConfig(newConfig, Path.Combine(targetPlatformDir, platformConfigFileName));

                            // create default dev config
                            var targetConfigDir = Path.Combine(settingsSourceDir, configDirName, defaultConfigName);

                            if (!Directory.Exists(targetConfigDir))
                            {
                                Util.EnsureDir(targetConfigDir);
                                Util.EnsureDir(Path.Combine(targetConfigDir, "Assets", "Resources"));
                                // create a default config
                                EditorHelper.SaveAppConfig(new AppConfig(),
                                    Path.Combine(targetConfigDir, "Assets", "Resources", gameConfigFileName));
                            }
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
                ApplyConfig(newIndex);
            }
            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(32)))
            {
                showCreatePanel = !showCreatePanel;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ApplyConfig(int newIndex)
        {
            string[] names = GetPlatformAndConfigName(currentIndexOfConfig);
            string platformTemplateDir;
            string configTemplateDir;
            string commonTemplateDir = Path.Combine(settingsSourceDir, commonDirName);

            string[] newNames = GetPlatformAndConfigName(newIndex);
            bool platformChanged = names == null || names[0] != newNames[0];

            if (names != null)
            {
                // Remove Plugins dir
                Util.ClearDir("Assets/Plugins");

                platformTemplateDir = Path.Combine(settingsSourceDir, platformDirName, names[0]);
                configTemplateDir = Path.Combine(settingsSourceDir, configDirName, names[1]);
                EditorHelper.ClearTemplateFiles(platformTemplateDir);
                EditorHelper.ClearTemplateFiles(configTemplateDir);
                if (platformChanged)
                {
                    RemoveDeps();
                }
            }

            currentIndexOfConfig = newIndex;
            names = GetPlatformAndConfigName(currentIndexOfConfig);
            platformTemplateDir = Path.Combine(settingsSourceDir, platformDirName, names[0]);
            configTemplateDir = Path.Combine(settingsSourceDir, configDirName, names[1]);

            Util.CopyDir(commonTemplateDir, ".");
            Util.CopyDir(platformTemplateDir, ".");
            Util.CopyDir(configTemplateDir, ".");

            LoadConfigs();
            if (platformChanged)
            {
                AddDeps();
            }

            if (gameConfig.name != names[1])
            {
                gameConfig.name = names[1];
                EditorHelper.SaveAppConfig(gameConfig);
            }

            if (platformConfig.name != names[0])
            {
                platformConfig.name = names[0];
                EditorHelper.SavePlatformConfig(platformConfig);
            }

            if (platformChanged)
            {
                EditorApplication.OpenProject(".");
            }
            else
            {
                AssetDatabase.Refresh();
            }
        }

        private string[] GetPlatformAndConfigName(int index)
        {
            if (index < 0)
            {
                return null;
            }

            var configname = allConfigs[index];
            var parts = configname.Split('/');
            var platformname = parts[0].Trim();
            configname = parts[1].Trim();
            return new string[] { platformname, configname };
        }

        private void RemoveDeps()
        {
            if (platformConfig.dependencies != null && platformConfig.dependencies.Length > 0)
            {
                var all = string.Join(' ', platformConfig.dependencies);
                EditorHelper.Shell("openupm", $"remove {all}");
            }
        }

        private void AddDeps()
        {
            if (platformConfig.dependencies != null && platformConfig.dependencies.Length > 0)
            {
                var all = string.Join(' ', platformConfig.dependencies);
                EditorHelper.Shell("openupm", $"add {all}");
            }
        }
    }
}
