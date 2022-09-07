using System;
using UnityEditor;
using UnityEngine;

namespace Zes.Settings
{
    public class AppConfigPanel : SettingPanel
    {
        public override string Name => "Game";

        public override string DisplayName => "Game";

        public override string Description => "Game settings";

        private bool appFoldout = true;
        private bool commonFoldout = true;
        private bool patchFoldout = true;
        private bool javascriptFoldout = true;
        private bool bundleFoldout = true;

        public override void OnGUI()
        {
            RenderFoldout("Application", ref appFoldout, () =>
            {
                manager.appConfig.appName = TextField("App short name", manager.appConfig.appName);
                EditorGUILayout.LabelField("App display name", Application.productName);
            });

            EditorGUILayout.Space();

            RenderFoldout("Common Settings", ref commonFoldout, () =>
            {
                manager.appConfig.loginServer = TextField("Login server", manager.appConfig.loginServer);
                manager.appConfig.checkUpdate = BoolField("Check update", manager.appConfig.checkUpdate);
                manager.appConfig.allowGuest = BoolField("Allow guest", manager.appConfig.allowGuest);
                manager.appConfig.appLanguage = TextField("App language", manager.appConfig.appLanguage);
            });

            EditorGUILayout.Space();

            RenderFoldout("Patch Settings", ref patchFoldout, () =>
            {
                manager.appConfig.patchServer = TextField("Patch server", manager.appConfig.patchServer);
                manager.appConfig.minVersion = TextField("Minimun version", manager.appConfig.minVersion);
                manager.appConfig.patchDataPath = TextField("Patch data path", manager.appConfig.patchDataPath);
                manager.appConfig.versionInfoFile = TextField("Version info file", manager.appConfig.versionInfoFile);
                manager.appConfig.patchInfoFile = TextField("Patch info file", manager.appConfig.patchInfoFile);
            });

            EditorGUILayout.Space();

            RenderFoldout("Javascript Settings", ref javascriptFoldout, () =>
            {
                manager.appConfig.javascriptBundle = TextField("Javascript bundle", manager.appConfig.javascriptBundle);
                manager.appConfig.javascriptData = TextField("Javascript data", manager.appConfig.javascriptData);
                manager.appConfig.javascriptEntry = TextField("Javascript entry", manager.appConfig.javascriptEntry);
            });

            EditorGUILayout.Space();

            RenderFoldout("Bundle Settings", ref javascriptFoldout, () =>
            {
                manager.appConfig.configBundleName = TextField("Config bundle name", manager.appConfig.configBundleName);
                manager.appConfig.configBundlePath = TextField("Config bundle path", manager.appConfig.configBundlePath);
                manager.appConfig.languageBundleName = TextField("Language bundle name", manager.appConfig.languageBundleName);
                manager.appConfig.languageBundlePath = TextField("Language bundle path", manager.appConfig.languageBundlePath);
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
