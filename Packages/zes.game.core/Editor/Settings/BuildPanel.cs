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

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.LabelField("Import configuration files");
            using (new GUIIndent())
            {
                BuildConfigurations.excelsPath = EditorGUILayout.TextField("Excels", BuildConfigurations.excelsPath);
            }
        }
    }
}
