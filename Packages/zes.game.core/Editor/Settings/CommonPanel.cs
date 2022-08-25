namespace Zes.Settings
{
    public class CommonPanel : SettingPanel
    {
        public override string Name => "Common";

        public override string DisplayName => "Common";

        public override string Description => "Common settings";

        public override void OnGUI()
        {
            config.loginServer = TextField("Login server", config.loginServer);
            config.allowGuest = BoolField("Allow guest", config.allowGuest);
            config.appLanguage = TextField("App language", config.appLanguage);
            config.checkUpdate = BoolField("Check update", config.checkUpdate);
        }
    }
}
