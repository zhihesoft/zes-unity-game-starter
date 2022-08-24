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
        public static string persistentDataPath => Path.Combine(Application.persistentDataPath, config.patchDataPath);
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

        public AppConfig appConfig;
        public AppInit appInit;


        private async Task InitJavascriptEnv()
        {
            if (jsEnv != null)
            {
                jsEnv.Dispose();
                jsEnv = null;
            }

            loader.UnloadBundle(config.javascriptBundle);
            await loader.LoadBundle(config.javascriptBundle);

#if UNITY_EDITOR
            var env = new JsEnv(new JSLoaderForEditor());
            string script = config.javascriptEntryEditor;
#else
            var env = new JsEnv(new JSLoaderForBundle());
            string script = config.javascriptEntryRuntime;
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
