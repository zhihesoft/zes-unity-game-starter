using UnityEditor;
using UnityEngine;
using Zes.Builders;

namespace Zes.Settings
{
    public class BuildPanel : SettingPanel
    {
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
            using (new GUIIndent())
            {
                BuildConfigurations.excelsPath = EditorGUILayout.TextField("Excels", BuildConfigurations.excelsPath);
                BuildApp.outputDir = EditorGUILayout.TextField("Output Dir", BuildApp.outputDir);
                EditorGUILayout.LabelField("Build no.", buildNo.ToString());

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Button("Build", GUILayout.Width(64), GUILayout.Height(32));
                }
            }
        }
    }
}
