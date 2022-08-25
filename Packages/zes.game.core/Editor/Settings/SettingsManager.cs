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
        public const string configPath = "Assets/boot.json";

        int currentIndexOfBaseConfig;
        int currentIndexOfPlatformBuildConfig;
        string[] allBaseConfig;
        string[] allPlatformConfig;


        public SettingsManager()
        {
        }

        public virtual void Initialize()
        {
            allBaseConfig = new DirectoryInfo(Path.Combine("GameSettings", "configs")).GetDirectories().Select(d => d.Name).ToArray();
            allPlatformConfig = new DirectoryInfo(Path.Combine("GameSettings", "platforms")).GetDirectories().Select(d => d.Name).ToArray();

            if (!File.Exists(configPath))
            {
                config = new T();
                SaveConfig();
            }
            else
            {
                LoadConfig();
            }
            InitializePanels();
            currentPanel = panels.Values.First();
            currentPanel.OnShow();
        }

        protected Dictionary<string, SettingPanel> panels = new Dictionary<string, SettingPanel>();

        protected SettingPanel currentPanel;

        protected T config;

        public void OnGUI()
        {
            ConfigSelector();
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(120), GUILayout.ExpandHeight(true)))
                {
                    foreach (var item in panels.Values)
                    {
                        Color old = GUI.backgroundColor;
                        GUI.backgroundColor = item == currentPanel ? Color.green : old;
                        if (GUILayout.Button(item.DisplayName, GUILayout.Height(32)))
                        {
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
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    if (currentPanel == null)
                    {
                        return;
                    }

                    EditorGUILayout.LabelField(currentPanel.Description);
                    currentPanel.OnGUI();
                    if (currentPanel.dirty)
                    {
                        SaveConfig();
                        currentPanel.dirty = false;
                    }
                }
            }
        }

        protected virtual void SaveConfig()
        {
            var json = JsonUtility.ToJson(config);
            File.WriteAllText(configPath, json, Util.Utf8WithoutBOM());
            AssetDatabase.Refresh();
        }

        protected virtual void LoadConfig()
        {
            var content = File.ReadAllText(configPath);
            config = JsonUtility.FromJson<T>(content);
        }

        protected virtual void InitializePanels()
        {
            panels.Clear();
            AddPanel(new CommonPanel(config));
            AddPanel(new PatchPanel(config));

        }

        private void AddPanel(SettingPanel panel)
        {
            panels.Add(panel.Name, panel);
        }

        private void ConfigSelector()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                currentIndexOfBaseConfig = EditorGUILayout.Popup(currentIndexOfBaseConfig, allBaseConfig);
                currentIndexOfPlatformBuildConfig = EditorGUILayout.Popup(currentIndexOfPlatformBuildConfig, allPlatformConfig);
                if (GUILayout.Button("Apply", GUILayout.Width(64)))
                {
                    // ApplyConfigs(allBaseConfig[currentIndexOfBaseConfig], allPlatformConfig[currentIndexOfPlatformBuildConfig]);
                }
                //using (new EditorGUILayout.HorizontalScope())
                //{
                //    GUILayout.FlexibleSpace();
                //}
                //EditorGUILayout.Space();
            }

        }

    }
}
