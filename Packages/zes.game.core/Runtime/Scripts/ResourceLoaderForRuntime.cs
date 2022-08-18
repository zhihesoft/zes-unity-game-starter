using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private Dictionary<string, string> assets2Bundle = new Dictionary<string, string>();
        private Dictionary<string, string> scenes2Bundle = new Dictionary<string, string>();

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

        protected override async Task<UnityEngine.Object> LoadAssetProc(string path, Type type)
        {
            Debug.Assert(assets2Bundle.ContainsKey(path), $"cannot find a bundle contain path: ({path})");
            var bundlename = assets2Bundle[path];
            var bundle = bundles[bundlename];
            Debug.Assert(bundle != null, $"bundle {bundlename} cannot be null");
            if (bundle.pending)
            {
                await bundle.Wait();
            }
            var op = bundle.item.LoadAssetAsync(path, type);
            await Util.WaitAsyncOperation(op);
            return op.asset;
        }

        protected override async Task<AssetBundle> LoadBundleProc(string name, Action<float> progress)
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

            string[] itemnames = bundle.isStreamedSceneAssetBundle ? bundle.GetAllScenePaths() : bundle.GetAllAssetNames();
            Dictionary<string, string> targetMap = bundle.isStreamedSceneAssetBundle ? scenes2Bundle : assets2Bundle;
            itemnames.ToList().ForEach(i => targetMap.Add(i.ToLower(), name));

            logger.Debug($"bundle {name} loaded");
            return bundle;
        }

        protected override void UnloadBundleProc(string name)
        {
            if (!bundles.ContainsKey(name))
            {
                return;
            }
            var bundle = bundles[name];
            bundles.Remove(name);
            bundle.item.Unload(true);
            assets2Bundle = assets2Bundle.Where(i => i.Value != name).ToDictionary(i => i.Key, i => i.Value);
            scenes2Bundle = scenes2Bundle.Where(i => i.Value != name).ToDictionary(i => i.Key, i => i.Value);
        }
    }
}
