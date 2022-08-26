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
        private bool scriptFoldout = true;

        public override void OnGUI()
        {
            commonFoldout = EditorGUILayout.Foldout(commonFoldout, "Common Settings", true);
            if (commonFoldout)
            {
                using (new GUIIndent())
                {
                    manager.gameConfig.loginServer = TextField("Login server", manager.gameConfig.loginServer);
                    manager.gameConfig.patchServer = TextField("Patch server", manager.gameConfig.patchServer);
                    manager.gameConfig.checkUpdate = BoolField("Check update", manager.gameConfig.checkUpdate);
                    manager.gameConfig.allowGuest = BoolField("Allow guest", manager.gameConfig.allowGuest);
                    manager.gameConfig.appLanguage = TextField("App language", manager.gameConfig.appLanguage);
                }
            }

            EditorGUILayout.Space();

            patchFoldout = EditorGUILayout.Foldout(patchFoldout, "Patch Settings", true);
            if (patchFoldout)
            {
                using (new GUIIndent())
                {
                    manager.gameConfig.minVersion = TextField("Minimun version", manager.gameConfig.minVersion);
                    manager.gameConfig.patchDataPath = TextField("Patch data path", manager.gameConfig.patchDataPath);
                    manager.gameConfig.patchInfoFile = TextField("Patch info file", manager.gameConfig.patchInfoFile);
                    manager.gameConfig.versionInfoFile = TextField("Version info file", manager.gameConfig.versionInfoFile);
                }
            }
            EditorGUILayout.Space();
            scriptFoldout = EditorGUILayout.Foldout(scriptFoldout, "Script Settings", true);
            if (scriptFoldout)
            {
                using (new GUIIndent())
                {
                    manager.gameConfig.javascriptBundle = TextField("JS bundle name", manager.gameConfig.javascriptBundle);
                    manager.gameConfig.javascriptEntryRuntime = TextField("JS Entry (Runtime)", manager.gameConfig.javascriptEntryRuntime);
                    manager.gameConfig.javascriptEntryEditor = TextField("JS Entry (Editor)", manager.gameConfig.javascriptEntryEditor);
                }
            }


        }
    }
}
