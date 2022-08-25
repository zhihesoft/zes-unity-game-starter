using Puerts;
using System.Threading.Tasks;
using UnityEngine;

namespace Zes
{
    public class JSLoaderForBundle : ILoader
    {
        private ILoader defaultLoader = new DefaultLoader();
        private const string puertsPrefix = "puerts";
        private const string jsbundle = "jsbundle";
        private static Logger logger = Logger.GetLogger<JSLoaderForBundle>();
        private string scripts = "";

        public async Task<bool> Init(ResourceLoader loader)
        {
            // TODO: add init
            //var bundle = await loader.LoadBundle(jsbundle);
            //var req = await loader.LoadAsset(bundle, App.config.javascriptPathRuntime, typeof(TextAsset));
            //var text = req as TextAsset;
            //scripts = text.text;
            //if (string.IsNullOrEmpty(scripts))
            //{
            //    logger.Error($"cannot find {App.config.javascriptPathRuntime}");
            //    return false;
            //}
            return true;
        }

        public bool FileExists(string filepath)
        {
            if (filepath.StartsWith(puertsPrefix))
            {
                return defaultLoader.FileExists(filepath);
            }

            return true;
        }

        public string ReadFile(string filepath, out string debugpath)
        {
            if (filepath.StartsWith(puertsPrefix))
            {
                return defaultLoader.ReadFile(filepath, out debugpath);
            }
            debugpath = filepath;
            return scripts;
        }
    }
}
