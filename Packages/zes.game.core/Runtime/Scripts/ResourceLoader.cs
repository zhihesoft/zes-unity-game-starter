using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zes
{
    public abstract class ResourceLoader
    {
        protected Dictionary<string, PendingItem<AssetBundle>> bundles = new Dictionary<string, PendingItem<AssetBundle>>();
        protected Dictionary<string, PendingItem<UnityEngine.Object>> assets = new Dictionary<string, PendingItem<UnityEngine.Object>>();

        public async Task LoadBundles(string[] names, Action<float> progress)
        {
            Debug.Assert(names.Length > 0);
            var total = names.ToDictionary<string, string, float>(i => i, i => 0);
            await Task.WhenAll(names.Select(name => LoadBundle(name, (p) =>
            {
                total[name] = p;
                progress?.Invoke(Math.Max(1, total.Sum(i => i.Value) / names.Length));
            })).ToArray());
        }

        public async Task LoadBundle(string name, Action<float> progress = null)
        {
            if (bundles.TryGetValue(name, out var bundle))
            {
                if (bundle.pending)
                {
                    await bundle.Wait();
                }
                progress?.Invoke(1);
                return;
            }
            bundle = new PendingItem<AssetBundle>();
            bundles.Add(name, bundle);
            var item = await LoadBundleProc(name, progress);
            bundle.Set(item);
        }

        public async Task<UnityEngine.Object> LoadAsset(string path, Type type)
        {
            if (assets.TryGetValue(path, out var asset))
            {
                if (asset.pending)
                {
                    await asset.Wait();
                }
                return asset.item;
            }
            asset = new PendingItem<UnityEngine.Object>();
            assets.Add(path, asset);
            var item = await LoadAssetProc(path, type);
            asset.Set(item);
            return item;
        }

        public void UnloadBundle(string name)
        {
            if (bundles.ContainsKey(name))
            {
                bundles.Remove(name);
            }
            UnloadBundleProc(name);
        }

        public abstract Task<Scene> LoadScene(string name, bool additive, Action<float> progress);
        public abstract Task<string> LoadText(string path);
        protected abstract Task<AssetBundle> LoadBundleProc(string name, Action<float> progress);
        protected abstract Task<UnityEngine.Object> LoadAssetProc(string path, Type type);
        protected abstract void UnloadBundleProc(string name);
    }
}
