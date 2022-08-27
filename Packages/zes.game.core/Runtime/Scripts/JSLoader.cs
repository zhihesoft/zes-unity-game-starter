using Puerts;
using System.Threading.Tasks;
using UnityEngine;

namespace Zes
{
    public abstract class JSLoader : ILoader
    {
        public static async Task<JSLoader> GetLoader()
        {
#if UNITY_EDITOR
            var loader = new JSLoaderForEditor();
#else
            var loader = new JSLoaderForRuntime();
#endif
            var ret = await loader.Init();
            Debug.Assert(ret, $"js loader init failed");
            return loader;
        }

        protected const string puerPrefix = "puerts";

        protected ILoader puerLoader = new DefaultLoader();

        protected Logger logger = Logger.GetLogger<JSLoader>();

        public abstract Task<bool> Init();
        public abstract void Dispose();
        protected abstract bool CustomFileExists(string filepath);
        protected abstract string CustomReadFile(string filepath, out string debugpath);

        public bool FileExists(string filepath)
        {
            if (filepath.StartsWith(puerPrefix))
            {
                return puerLoader.FileExists(filepath);
            }
            return CustomFileExists(filepath);
        }

        public string ReadFile(string filepath, out string debugpath)
        {
            if (filepath.StartsWith(puerPrefix))
            {
                return puerLoader.ReadFile(filepath, out debugpath);
            }

            return CustomReadFile(filepath, out debugpath);
        }

    }
}
