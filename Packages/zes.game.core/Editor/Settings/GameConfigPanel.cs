using System;
using UnityEditor;

namespace Zes.Settings
{
    public class GameConfigPanel : SettingPanel
    {
        public override string Name => "Game";

        public override string DisplayName => "Game";

        public override string Description => "Game settings";

        private bool appFoldout = true;
        private bool commonFoldout = true;
        private bool patchFoldout = true;

        public override void OnGUI()
        {
            RenderFoldout("Application", ref appFoldout, () =>
            {
                manager.gameConfig.appShortName = TextField("App short name", manager.gameConfig.appShortName);
                manager.gameConfig.appDisplayName = TextField("App display name", manager.gameConfig.appDisplayName);
            });

            EditorGUILayout.Space();

            RenderFoldout("Common Settings", ref commonFoldout, () =>
            {
                manager.gameConfig.loginServer = TextField("Login server", manager.gameConfig.loginServer);
                manager.gameConfig.checkUpdate = BoolField("Check update", manager.gameConfig.checkUpdate);
                manager.gameConfig.allowGuest = BoolField("Allow guest", manager.gameConfig.allowGuest);
                manager.gameConfig.appLanguage = TextField("App language", manager.gameConfig.appLanguage);
            });

            EditorGUILayout.Space();

            RenderFoldout("Patch Settings", ref patchFoldout, () =>
            {
                manager.gameConfig.patchServer = TextField("Patch server", manager.gameConfig.patchServer);
                manager.gameConfig.minVersion = TextField("Minimun version", manager.gameConfig.minVersion);
            });

        }

        private void RenderFoldout(string name, ref bool foldout, Action act)
        {
            foldout = EditorGUILayout.Foldout(foldout, name, true);
            if (foldout)
            {
                using (new GUIIndent())
                {
                    act();
                }
            }
        }
    }
}
