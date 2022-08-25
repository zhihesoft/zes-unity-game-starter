namespace Zes.Settings
{
    public class TypescriptPanel : SettingPanel
    {
        public override string Name => "Typescript";

        public override string DisplayName => "Typescript";

        public override string Description => "Typescript settings";

        public override void OnGUI()
        {
            config.typescriptProjectPath = TextField("Typescript project path", config.typescriptProjectPath);
            config.javascriptEntryEditor = TextField("Javascript entry (Editor)", config.javascriptEntryEditor);
            config.javascriptEntryRuntime = TextField("Javascript entry (Runtime)", config.javascriptEntryRuntime);
        }
    }
}
