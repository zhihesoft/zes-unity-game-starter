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
        public const string platformConfigPath = "platform.json";

        protected Dictionary<string, SettingPanel> panels = new Dictionary<string, SettingPanel>();
        protected SettingPanel currentPanel;
        protected T gameConfig;
        protected PlatformConfig platformConfig;
        private string[] allConfigs;
        private int currentIndexOfConfig = -1;
        private bool showAddPanel = false;
        private string newConfigName = "";
        private string newPlatformName = "";

        public virtual void Initialize()
        {
            InitializePanels();
            currentPanel = panels.Values.First();
            currentPanel.config = gameConfig;
            currentPanel.OnShow();

            var configs = new DirectoryInfo(Path.Combine(settingsSourceDir, "configs")).GetDirectories().Select(d => d.Name).ToArray();
            var platforms = new DirectoryInfo(Path.Combine(settingsSourceDir, "platforms")).GetDirectories().Select(d => d.Name).ToArray();
            allConfigs = platforms.SelectMany(p => configs.Select(c => $"{p}/{c}")).ToArray();

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
            DrawAddPanel();

            if (!IsConfigValid())
            {
                EditorGUILayout.LabelField("Config invalid !!",
                    EditorStyles.centeredGreyMiniLabel,
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                return;
            }

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
                            AssetDatabase.Refresh();
                        }
                    }
                }
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
            if (!File.Exists(platformConfigPath))
                return false;
            platformConfig = PlatformConfig.load();
            panels.Values.ToList().ForEach(i => i.platformConfig = platformConfig);
            return true;
        }

        protected void SavePlatformConfig()
        {
            SavePlatformConfig(platformConfig, platformConfigPath);
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

        private void DrawAddPanel()
        {
            if (!showAddPanel)
            {
                return;
            }

            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    newConfigName = EditorGUILayout.TextField(newConfigName);
                    if (GUILayout.Button("Create new game config", GUILayout.Width(200)))
                    {
                        if (!string.IsNullOrEmpty(newConfigName))
                        {
                            var newConfig = new AppConfig();
                            newConfig.name = newConfigName.Trim();
                            var targetdir = Path.Combine(settingsSourceDir, "configs", newConfig.name, "Resources");
                            Util.EnsureDir(targetdir);
                            var targetpath = Path.Combine(targetdir, "boot.json");
                            SaveGameConfig(newConfig, targetpath);
                            EditorUtility.DisplayDialog("DONE", $"game config {newConfigName} created", "OK");
                            newConfigName = "";
                        }
                    }
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    newPlatformName = EditorGUILayout.TextField(newPlatformName);
                    if (GUILayout.Button("Create new platform", GUILayout.Width(200)))
                    {
                        if (!string.IsNullOrEmpty(newPlatformName))
                        {
                            var newConfig = new PlatformConfig();
                            newConfig.name = newPlatformName.Trim();
                            var targetdir = Path.Combine(settingsSourceDir, "platforms", newConfig.name);
                            Util.EnsureDir(targetdir);
                            var targetpath = Path.Combine(targetdir, "platform.json");
                            SavePlatformConfig(newConfig, targetpath);
                            EditorUtility.DisplayDialog("DONE", $"platform {newPlatformName} created", "OK");
                            newPlatformName = "";
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
                showAddPanel = !showAddPanel;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ApplyConfig()
        {
            var configpath = allConfigs[currentIndexOfConfig];
            var parts = configpath.Split('/');
            var platformpath = parts[0].Trim();
            configpath = parts[1].Trim();

            // remove Assets/csc.rsp
            if (File.Exists("Assets/csc.rsp"))
            {
                File.Delete("Assets/csc.rsp");
            }

            Util.ClearDir(settingsTargetDir);
            var configsourcedir = Path.Combine(settingsSourceDir, "configs", configpath);
            var configtargetdir = settingsTargetDir;
            Util.CopyDir(configsourcedir, configtargetdir);

            var platformsourcedir = Path.Combine(settingsSourceDir, "platforms", platformpath);
            Util.CopyDir(platformsourcedir, ".");

            LoadGameConfig();
            LoadPlatformConfig();

            if (gameConfig.name != configpath)
            {
                gameConfig.name = configpath;
                SaveGameConfig();
            }
            if (platformConfig.name != platformpath)
            {
                platformConfig.name = platformpath;
                SavePlatformConfig();
            }

            AssetDatabase.Refresh();
        }

    }
}
