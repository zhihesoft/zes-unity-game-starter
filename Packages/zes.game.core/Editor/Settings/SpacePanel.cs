using UnityEngine;

namespace Zes.Settings
{
    public class SpacePanel : SettingPanel
    {
        public SpacePanel(string name)
        {
            this._name = name;
        }

        private string _name = string.Empty;

        public override string Name => _name;

        public override string DisplayName => "";

        public override string Description => "";

        public override void OnGUI()
        {
            GUILayout.FlexibleSpace();
        }
    }
}
