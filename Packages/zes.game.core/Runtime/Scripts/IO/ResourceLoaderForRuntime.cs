using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Zes.IO
{
    public class ResourceLoaderForRuntime : ResourceLoader
    {
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
            logger.Debug($"bundlepath is {bundlePath}");
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

        public override async Task UnloadScene(Scene scene)
        {
            var op = SceneManager.UnloadSceneAsync(scene);
            await Util.WaitAsyncOperation(op);
        }
    }
}
