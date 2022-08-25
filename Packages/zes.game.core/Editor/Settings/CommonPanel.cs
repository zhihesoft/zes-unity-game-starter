namespace Zes.Settings
{
    public class CommonPanel : SettingPanel
    {
        public CommonPanel(AppConfig config) : base(config)
        {
        }

        public override string Name => "Common";

        public override string DisplayName => "Common";

        public override string Description => "Common settings";

        public override void OnGUI()
        {
            config.appLanguage = TextField("App language", config.appLanguage);
        }
    }
}
