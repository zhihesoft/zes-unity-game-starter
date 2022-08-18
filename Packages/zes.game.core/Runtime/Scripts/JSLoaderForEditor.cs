using Puerts;
using System.IO;
using System.Text;

namespace Zes
{
    public class JSLoaderForEditor : ILoader
    {
        private ILoader defaultLoader = new DefaultLoader();
        private string sourcePath = App.config.javascriptPathEditor;
        private const string puertsPrefix = "puerts";
        private static Logger logger = Logger.GetLogger<JSLoaderForEditor>();

        public bool FileExists(string filepath)
        {
            if (filepath.StartsWith(puertsPrefix))
            {
                return defaultLoader.FileExists(filepath);
            }

            var path = Path.Combine(sourcePath, filepath);
            bool ret = File.Exists(path);
            return ret;
        }

        public string ReadFile(string filepath, out string debugpath)
        {
            if (filepath.StartsWith(puertsPrefix))
            {
                return defaultLoader.ReadFile(filepath, out debugpath);
            }

            var path = Path.Combine(sourcePath, filepath);
            debugpath = path;
            logger.debug($"load js file: {path}");
            return File.ReadAllText(path, Encoding.UTF8);
        }
    }
}

