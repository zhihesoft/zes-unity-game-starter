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


        public AppInit appInit;
        public TextAsset appConfigFile;

        private AppConfig appConfig;
        private JsEnv jsEnv;
        private JSLoader jsLoader;
        private Logger logger = Logger.GetLogger<App>();

        private string javascriptEntry
        {
            get
            {
#if UNITY_EDITOR
#endif
                return "";
            }
        }


        private async Task InitJavascriptEnv()
        {
            if (jsEnv != null)
            {
                jsEnv.Dispose();
                jsEnv = null;
            }

            if (jsLoader != null)
            {
                jsLoader.Dispose();
            }

            jsLoader = await JSLoader.GetLoader();

            var env = new JsEnv(jsLoader);

            env.UsingAction<bool>();
            env.UsingAction<float>();
            env.UsingAction<string>();
            env.UsingAction<string, string>();
            env.UsingAction<Vector2>();
            env.UsingAction<Vector3>();
            env.UsingFunc<string, string>();

            appInit?.OnInit(env);

            env.Eval($"require('{appConfig.javascriptEntry}');");
            jsEnv = env;
        }

        public async void Restart()
        {
            await InitJavascriptEnv();
        }

        private void Start()
        {
            Debug.Assert(appConfigFile != null, "boot config cannot be null");
            DontDestroyOnLoad(gameObject);

            instance = this;
            appInit?.BeforeInit();
            appConfig = JsonUtility.FromJson<AppConfig>(appConfigFile.text);
            loader = ResourceLoader.GetLoader();
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
