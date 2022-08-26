namespace Zes.Settings
{
    public class PatchPanel : SettingPanel
    {

        public override string Name => "Patch";

        public override string DisplayName => "Patch";

        public override string Description => "Patch Settings";

        public override void OnGUI()
        {
            config.patchServer = TextField("Patch server", config.patchServer);
            config.minVersion = TextField("Minimun version", config.minVersion);
        }
    }
}
