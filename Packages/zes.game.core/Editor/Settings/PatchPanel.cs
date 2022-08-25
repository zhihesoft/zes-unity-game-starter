using UnityEditor;

namespace Zes.Settings
{
    public class PatchPanel : SettingPanel
    {
        public PatchPanel(AppConfig config) : base(config)
        {
        }

        public override string Name => "Patch";

        public override string DisplayName => "Patch";

        public override string Description => "Patch Settings";

        public override void OnGUI()
        {
            config.patchDataPath = TextField("patch data path", config.patchDataPath);
        }
    }
}
