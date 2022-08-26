namespace Zes.Settings
{
    public class PlatformPanel : SettingPanel
    {
        public override string Name => "Platform";

        public override string DisplayName => "Platform";

        public override string Description => "Platform settings";

        public override void OnGUI()
        {
            manager.platformConfig.androidKeystorePassword = TextField("Android keystore pwd", manager.platformConfig.androidKeystorePassword);
            manager.platformConfig.androidKeyAliasPassword = TextField("Android keyalias pwd", manager.platformConfig.androidKeyAliasPassword);
        }
    }
}
