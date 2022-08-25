using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Zes
{
    public class ResourceLoaderForRuntime : ResourceLoader
    {
        static Logger logger = Logger.GetLogger<ResourceLoaderForRuntime>();

        public override async Task<Scene> LoadScene(string name, bool additive, Action<float> progress)
        {
            Scene loadedScene = default(Scene);
            UnityAction<Scene, LoadSceneMode> loadCallback = (scene, mode) =>
            {
                loadedScene = scene;
            };
            SceneManager.sceneLoaded += loadCallback;
            var op = SceneManager.LoadSceneAsync(name, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            await Util.WaitAsyncOperation(op, progress);
            SceneManager.sceneLoaded -= loadCallback;
            return loadedScene;
        }

        public override async Task<string> LoadText(string path)
        {
            byte[] bytes;
            if (path.StartsWith("jar:"))
            {
                var webrequest = UnityWebRequest.Get(path);
                webrequest.downloadHandler = new DownloadHandlerBuffer();
                var req = webrequest.SendWebRequest();
                await Util.WaitAsyncOperation(req);
                bytes = webrequest.downloadHandler.data;
            }
            else
            {
                if (!File.Exists(path))
                {
                    logger.Error($"{path} not found");
                    return "";
                }
                bytes = File.ReadAllBytes(path);
            }
            string str = Encoding.UTF8.GetString(bytes);
            return str;
        }

        public override async Task<UnityEngine.Object> LoadAsset(AssetBundle bundle, string path, Type type)
        {
            Debug.Assert(bundle != null, $"bundle cannot be null");
            var op = bundle.LoadAssetAsync(path, type);
            await Util.WaitAsyncOperation(op);
            return op.asset;
        }

        public override async Task<AssetBundle> LoadBundle(string name, Action<float> progress)
        {
            name = name.ToLower();
            logger.Debug($"begin to load bundle {name}");
            string bundlePath = Path.Combine(App.persistentDataPath, name);
            if (!File.Exists(bundlePath))
            {
                bundlePath = Path.Combine(Application.streamingAssetsPath, name);
            }
            Debug.Assert(File.Exists(bundlePath), $"cannot find bundle in path: ({bundlePath})");
            var bundlereq = AssetBundle.LoadFromFileAsync(bundlePath);
            await Util.WaitAsyncOperation(bundlereq, progress);
            var bundle = bundlereq.assetBundle;
            Debug.Assert(bundle != null, $"load bundle {name} failed, get null bundle");
            logger.Debug($"bundle {name} loaded");
            return bundle;
        }

        public override void UnloadBundle(AssetBundle bundle)
        {
            bundle.Unload(true);
        }
    }
}
