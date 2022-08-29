using UnityEditor;
using UnityEngine;
using Zes.Builders;

namespace Zes.Settings
{
    public class BuildPanel : SettingPanel
    {
        private Logger logger = Logger.GetLogger<BuildPanel>();
        public override string Name => "Build";

        public override string DisplayName => "Build";

        public override string Description => "Build project";

        private int buildNo = 0;

        public override void OnShow()
        {
            base.OnShow();
            buildNo = BuildNo.Get();
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("Platform", manager.platformConfig.name);
            EditorGUILayout.LabelField("App config", manager.appConfig.name);
            EditorGUILayout.LabelField("Build no.", buildNo.ToString());
            EditorGUILayout.Space();

            BuildConfigurations.excelsPath = EditorGUILayout.TextField("Excels", BuildConfigurations.excelsPath);
            BuildApp.outputDir = EditorGUILayout.TextField("Output Dir", BuildApp.outputDir);
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Build Bundles", GUILayout.Width(128), GUILayout.Height(32)))
                {
                    bool buildsucc = BuildRunner.Run(EditorUserBuildSettings.activeBuildTarget,
                        new BuildJavascript(),
                        new BuildConfigurations(),
                        new BuildBundle());
                    if (!buildsucc)
                    {
                        return;
                    }
                    buildNo = BuildNo.Inc();
                }
                if (GUILayout.Button("Build", GUILayout.Width(64), GUILayout.Height(32)))
                {
                    bool buildsucc = BuildRunner.Run(EditorUserBuildSettings.activeBuildTarget,
                        new BuildJavascript(),
                        new BuildConfigurations(),
                        new BuildBundle(),
                        new BuildApp());
                    if (!buildsucc)
                    {
                        return;
                    }
                    buildNo = BuildNo.Inc();
                }
                EditorGUILayout.Space();
            }
            EditorGUILayout.Space();
        }
    }
}
