namespace Zes.Settings
{
    public class PlatformPanel : SettingPanel
    {
        public override string Name => "Platform";

        public override string DisplayName => "Platform";

        public override string Description => "Platform settings";

        public override void OnGUI()
        {
            platformConfig.androidKeystorePassword = TextField("Android keystore pwd", platformConfig.androidKeystorePassword);
            platformConfig.androidKeyAliasPassword = TextField("Android keyalias pwd", platformConfig.androidKeyAliasPassword);
        }
    }
}
