using UnityEditor;

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
                    manager.gameConfig.loginServer = TextField("Login server", manager.gameConfig.loginServer);
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
                    manager.gameConfig.patchServer = TextField("Patch server", manager.gameConfig.patchServer);
                    manager.gameConfig.minVersion = TextField("Minimun version", manager.gameConfig.minVersion);
                }
            }
        }
    }
}
