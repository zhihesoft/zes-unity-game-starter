using UnityEditor;

namespace Zes.Builders
{
    public abstract class BuildTask
    {
        public BuildTask(AppConstants constants, BuildTarget target)
        {
            this.constants = constants;
            this.target = target;
        }

        public abstract string name { get; }

        protected readonly Logger logger = Logger.GetLogger<BuildTask>();
        protected readonly AppConstants constants;
        protected readonly BuildTarget target;

        public bool Build()
        {
            if (!BeforeBuild())
            {
                logger.Error($"{name} before build failed");
                return false;
            }

            if (!OnBuild())
            {
                logger.Error($"{name} build failed");
                Cleanup();
                return false;
            }

            AfterBuild();
            Cleanup();
            logger.Info($"Build:{name} finished. {FinishInfo()}");
            return true;

        }

        protected abstract bool BeforeBuild();

        protected abstract bool OnBuild();

        protected abstract void AfterBuild();

        protected virtual void Cleanup() { }

        protected virtual string FinishInfo()
        {
            return "";
        }
    }
}
