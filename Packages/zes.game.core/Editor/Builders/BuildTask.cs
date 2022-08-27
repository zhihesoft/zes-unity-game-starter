namespace Zes.Builders
{
    public abstract class BuildTask
    {
        public BuildTask(AppConstants constants)
        {
            this.constants = constants;
        }

        public abstract string name { get; }

        protected readonly Logger logger = Logger.GetLogger<BuildTask>();
        protected readonly AppConstants constants;

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
