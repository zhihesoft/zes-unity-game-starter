using Puerts;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Zes
{
    /// <summary>
    /// ZES App
    /// </summary>
    public class App : MonoBehaviour
    {
        public static AppConfig config => instance.appConfig;
        public static AppProp prop => instance.appProp;
        public static string persistentDataPath => Path.Combine(Application.persistentDataPath, prop.patchDataPath);
        public static bool inEditor
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }
        public static ResourceLoader loader { get; private set; }

        private static App instance;
        private static Logger logger = Logger.GetLogger<App>();
        private static JsEnv jsEnv;

        public AppInit appInit;
        public AppProp appProp;
        protected AppConfig appConfig;
        private AssetBundle scriptBundle;

        private async Task InitJavascriptEnv()
        {
            if (jsEnv != null)
            {
                jsEnv.Dispose();
                jsEnv = null;
            }

            if (scriptBundle != null)
            {
                loader.UnloadBundle(scriptBundle);
            }
            scriptBundle = await loader.LoadBundle(appProp.javascriptBundle);

#if UNITY_EDITOR
            var env = new JsEnv(new JSLoaderForEditor());
            string script = appProp.javascriptEntryEditor;
#else
            var env = new JsEnv(new JSLoaderForBundle());
            string script = appProp.javascriptEntryRuntime;
#endif
            env.UsingAction<bool>();
            env.UsingAction<float>();
            env.UsingAction<string>();
            env.UsingAction<string, string>();
            env.UsingAction<Vector2>();
            env.UsingAction<Vector3>();
            env.UsingFunc<string, string>();

            appInit?.OnInit(env);

            env.Eval($"require('{script}');");

            jsEnv = env;
        }

        public async void Restart()
        {
            await InitJavascriptEnv();
        }

        private void Start()
        {
            logger.Info("App starting");
            instance = this;
            DontDestroyOnLoad(gameObject); // dont destroy
#if UNITY_EDITOR
            loader = new ResourceLoaderForEditor();
#else
            loader = new ResourceLoaderForRuntime();
#endif
            Restart();
        }

        private void Update()
        {
            if (jsEnv != null)
            {
                jsEnv.Tick();
            }
        }
    }
}
