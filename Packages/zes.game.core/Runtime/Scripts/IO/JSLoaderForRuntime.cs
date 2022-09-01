using System.Threading.Tasks;
using UnityEngine;

namespace Zes.IO
{
    public class JSLoaderForRuntime : JSLoader
    {
        private string scripts = "";
        private AssetBundle scriptBundle;


        public override async Task<bool> Init()
        {
            scriptBundle = await App.loader.LoadBundle(App.config.javascriptBundle);
            Debug.Assert(scriptBundle != null, $"cannot load script bundle: {App.config.javascriptBundle}.");
            var txt = (TextAsset)await App.loader.LoadAsset(scriptBundle, App.config.javascriptData, typeof(TextAsset));
            Debug.Assert(txt != null, $"cannot load script text ({App.config.javascriptData}) from bundle");
            scripts = txt.text;
            return true;
        }

        public override void Dispose()
        {
            if (scriptBundle != null)
            {
                App.loader.UnloadBundle(scriptBundle);
                scriptBundle = null;
            }
            scripts = string.Empty;
        }

        protected override bool CustomFileExists(string filepath)
        {
            return true;
        }

        protected override string CustomReadFile(string filepath, out string debugpath)
        {
            debugpath = filepath;
            return scripts;
        }
    }
}
