using System.IO;
using UnityEditor;
using UnityEngine;

namespace Zes.Settings
{
    public class GameConfigPanel : SettingPanel
    {
        public override string Name => "Game";

        public override string DisplayName => "Game";

        public override string Description => "Game settings";

        private bool commonFoldout = true;
        private bool patchFoldout = true;

        public override void OnGUI()
        {
            commonFoldout = EditorGUILayout.Foldout(commonFoldout, "Common Settings", true);
            if (commonFoldout)
            {
                using (new GUIIndent())
                {
                    config.loginServer = TextField("Login server", config.loginServer);
                    config.allowGuest = BoolField("Allow guest", config.allowGuest);
                    config.appLanguage = TextField("App language", config.appLanguage);
                    config.checkUpdate = BoolField("Check update", config.checkUpdate);
                }
            }

            EditorGUILayout.Space();

            patchFoldout = EditorGUILayout.Foldout(patchFoldout, "Patch Settings", true);
            if (patchFoldout)
            {
                using (new GUIIndent())
                {
                    config.patchServer = TextField("Patch server", config.patchServer);
                    config.minVersion = TextField("Minimun version", config.minVersion);
                }
            }

            GUILayout.FlexibleSpace();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Sync To Settings", GUILayout.Height(32)))
                {
                    SyncToSettings();
                }
            }

            EditorGUILayout.Space();
        }

        private void SyncToSettings()
        {
            var sourceDir = SettingsManager<AppConfig>.settingsSourceDir;
            string dir = Path.Combine(sourceDir, platformConfig.name, SettingsManager<AppConfig>.configSourcePathName, config.name, "Assets");
            string filepath = Path.Combine(dir, SettingsManager<AppConfig>.gameConfigFileName);
            File.Copy("Assets/boot.json", filepath, true);
            EditorUtility.DisplayDialog("DONE", "Copy finished", "OK");
        }
    }
}
