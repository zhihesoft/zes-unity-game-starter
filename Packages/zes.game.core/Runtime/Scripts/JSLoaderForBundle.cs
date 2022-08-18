using Puerts;
using System.Threading.Tasks;
using UnityEngine;

namespace Zes
{
    public class JSLoaderForBundle : ILoader
    {
        private ILoader defaultLoader = new DefaultLoader();
        private const string puertsPrefix = "puerts";
        private static Logger logger = Logger.GetLogger<JSLoaderForBundle>();
        private string scripts = "";

        public async Task<bool> Init(IResourceLoader resource)
        {
            var bundle = await resource.LoadBundle("data", null);
            var req = await resource.LoadAsset(bundle, "Assets/Bundles/data/main.mjs", typeof(TextAsset));
            var text = req as TextAsset;
            scripts = text.text;
            if (string.IsNullOrEmpty(scripts))
            {
                logger.error($"cannot find main.mjs");
                return false;
            }
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
