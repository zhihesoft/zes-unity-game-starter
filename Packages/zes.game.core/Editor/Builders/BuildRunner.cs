using UnityEditor;

namespace Zes.Builders
{
    public class BuildRunner
    {
        public static bool Run(BuildTarget target, params BuildTask[] tasks)
        {
            var runner = new BuildRunner(target);
            return runner.Run(tasks);
        }

        private BuildRunner(BuildTarget target)
        {
            this.target = target;
        }

        public AppConfig appConfig { get; private set; }
        public PlatformConfig platformConfig { get; private set; }
        public BuildTarget target { get; private set; }


        public bool Run(BuildTask[] tasks)
        {
            appConfig = EditorHelper.LoadAppConfig();
            platformConfig = EditorHelper.LoadPlatformConfig();

            foreach (var task in tasks)
            {
                if (!task.Build(this))
                    return false;
            }
            return true;
        }
    }
}
