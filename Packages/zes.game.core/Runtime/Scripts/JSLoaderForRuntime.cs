using System.Threading.Tasks;
using UnityEngine;

namespace Zes
{
    public class JSLoaderForRuntime : JSLoader
    {
        private string scripts = "";
        private AssetBundle scriptBundle;


        public override async Task<bool> Init()
        {
            scriptBundle = await App.loader.LoadBundle(App.constants.javascriptBundle);
            Debug.Assert(scriptBundle != null, $"cannot load script bundle: {App.constants.javascriptBundle}.");
            var txt = (TextAsset)await App.loader.LoadAsset(scriptBundle, App.constants.javascriptEntryRuntime, typeof(TextAsset));
            Debug.Assert(txt != null, $"cannot load script text ({App.constants.javascriptEntryRuntime}) from bundle");
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
