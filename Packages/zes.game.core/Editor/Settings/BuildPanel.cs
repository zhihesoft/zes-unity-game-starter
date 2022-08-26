using UnityEditor;
using UnityEngine;

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

            }
        }
    }
}
