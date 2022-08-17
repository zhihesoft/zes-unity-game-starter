using Puerts;
using System.Threading.Tasks;
using UnityEngine;

namespace Zes
{
    /// <summary>
    /// ZES App
    /// </summary>
    public abstract class App : MonoBehaviour
    {
        private static Logger logger = Logger.GetLogger<App>();

        public JsEnv env { get; private set; }
        public abstract string jsPath { get; }
        public bool inEditor { get; private set; }

        public async void Init()
        {
            await OnInit(jsPath);
        }

        public void Restart()
        {

        }

        protected abstract Task<bool> OnInit(string jsPath);


        private void Start()
        {
#if UNITY_EDITOR
            inEditor = true;
#endif
            logger.info($"App starting. ");
            DontDestroyOnLoad(gameObject); // dont destroy
            Init();
        }

        private void Update()
        {
            if (env != null)
            {
                env.Tick();
            }
        }
    }
}
