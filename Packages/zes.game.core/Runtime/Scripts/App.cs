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
        public static AppConstants constants => instance.appConstants;
        public static string persistentDataPath => Path.Combine(Application.persistentDataPath, constants.patchDataPath);
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
        public AppConstants appConstants;
        public TextAsset bootConfig;

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
            scriptBundle = await loader.LoadBundle(appConstants.javascriptBundle);

#if UNITY_EDITOR
            var env = new JsEnv(new JSLoaderForEditor());
            string script = appConstants.javascriptEntryEditor;
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
            Debug.Assert(bootConfig != null, "boot config cannot be null");

            logger.Info("App starting");
            DontDestroyOnLoad(gameObject); // dont destroy
            instance = this;
            appConfig = JsonUtility.FromJson<AppConfig>(bootConfig.text);
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
