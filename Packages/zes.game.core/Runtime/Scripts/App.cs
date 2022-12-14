using Puerts;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Zes.IO;
using Zes.Native;

namespace Zes
{
    /// <summary>
    /// ZES App
    /// </summary>
    public class App : MonoBehaviour
    {
        public static AppConfig config => instance.appConfig;
        public static NativeChannel native { get; private set; }
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

        public static string platform
        {
            get
            {
#if UNITY_ANDROID
                return "android";
#elif UNITY_IOS
                return "ios";
#else
                return Application.platform.ToString();
#endif
            }
        }

        public static ResourceLoader loader { get; private set; }
        private static App instance;

        public static async void Restart()
        {
            await instance.InitJavascriptEnv();
        }


        public AppInit appInit;

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
            env.UsingFunc<int, string>();

            appInit?.OnInit(env);

            var i18n = env.Eval<Func<int, string>>($"var m = require('{appConfig.javascriptEntry}'); m.i18n");
            I18n.translation = i18n;
            jsEnv = env;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            var txt = Resources.Load<TextAsset>("app");
            if (txt == null)
            {
                logger.Error($"Cannot find app.json in Resources directory");
                return;
            }
            appConfig = JsonUtility.FromJson<AppConfig>(txt.text);
            native = NativeChannel.Create();
            loader = ResourceLoader.GetLoader();
            instance = this;

            appInit?.OnStart();

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
