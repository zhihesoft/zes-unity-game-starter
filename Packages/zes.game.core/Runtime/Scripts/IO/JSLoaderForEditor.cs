#if UNITY_EDITOR
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Zes.IO
{
    public class JSLoaderForEditor : JSLoader
    {
        public override Task<bool> Init()
        {

            return Task.FromResult(true);
        }


        public override void Dispose()
        {
        }

        protected override bool CustomFileExists(string filepath)
        {
            var path = Path.Combine(filepath);
            bool ret = File.Exists(path);
            return ret;
        }

        protected override string CustomReadFile(string filepath, out string debugpath)
        {
            var path = Path.Combine(filepath);
            debugpath = path;
            // logger.Debug($"load js file: {path}");
            return File.ReadAllText(path, Encoding.UTF8);
        }
    }
}


#endif
