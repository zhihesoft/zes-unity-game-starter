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

        public AppInit appInit;
        public AppConstants appConstants;
        public TextAsset bootConfig;

        private AppConfig appConfig;
        private JsEnv jsEnv;
        private JSLoader jsLoader;
        private Logger logger = Logger.GetLogger<App>();

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

            env.Eval($"require('{constants.javascriptEntry}');");
            jsEnv = env;
        }

        public async void Restart()
        {
            await InitJavascriptEnv();
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            Debug.Assert(bootConfig != null, "boot config cannot be null");
            instance = this;
            appConfig = JsonUtility.FromJson<AppConfig>(bootConfig.text);
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
