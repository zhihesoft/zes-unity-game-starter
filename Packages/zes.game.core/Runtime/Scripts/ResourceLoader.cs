using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Zes
{
    public class ResourceLoader
    {
        private static Logger logger = Logger.GetLogger<ResourceLoader>();

        public ResourceLoader()
        {
        }

#if UNITY_EDITOR
        private IResourceLoader loader = new ResourceLoaderForEditor();
#else
        private IResourceLoader loader = new ResourceLoaderForRuntime();
#endif

        private Dictionary<string, PendingItem<AssetBundle>> bundles = new Dictionary<string, PendingItem<AssetBundle>>();
        private Dictionary<string, string> assets2Bundle = new Dictionary<string, string>();
        private Dictionary<string, string> scenes2Bundle = new Dictionary<string, string>();

        public async Task LoadBundle(string name, Action<float> progress = null)
        {
            if (bundles.ContainsKey(name))
            {
                var existed = bundles[name];
                if (existed.pending)
                {
                    await existed.Wait();
                }
                return;
            }

            var item = new PendingItem<AssetBundle>();
            bundles.Add(name, item);
            var bundle = await loader.LoadBundle(name, progress);
            item.SetDate(bundle);
            if (bundle != null)
            {
                string[] itemnames = bundle.isStreamedSceneAssetBundle ? bundle.GetAllScenePaths() : bundle.GetAllAssetNames();
                Dictionary<string, string> targetMap = bundle.isStreamedSceneAssetBundle ? scenes2Bundle : assets2Bundle;
                foreach (string itemname in itemnames)
                {
                    targetMap.Add(itemname.ToLower(), name);
                }
            }
        }

        public async Task LoadBundles(string[] names, Action<float> progress = null)
        {
            if (names.Length == 0)
            {
                progress?.Invoke(1);
                return;
            }

            var total = names.ToDictionary<string, string, float>(i => i, i => 0);
            await Task.WhenAll(names.Select(name => LoadBundle(name, (p) =>
            {
                total[name] = p;
                progress?.Invoke(Math.Max(1, total.Sum(i => i.Value) / names.Length));
            })).ToArray());
        }

        public async Task<UnityEngine.Object> LoadAsset(string path, Type type)
        {
            if (!assets2Bundle.TryGetValue(path, out string bundlename))
            {
                logger.error($"cannot find a bundle contain path: ({path})");
                return null;
            }
            await LoadBundle(bundlename);
            var bundle = bundles[bundlename];
            var ret = await loader.LoadAsset(bundle.item, path, type);
            return ret;
        }

        public void UnloadBundle(string name)
        {
            if (!bundles.ContainsKey(name))
            {
                return;
            }
            var bundle = bundles[name];
            bundles.Remove(name);
            if (bundle != null)
            {
                bundle.item.Unload(true);
            }
            assets2Bundle = assets2Bundle.Where(i => i.Value != name).ToDictionary(i => i.Key, i => i.Value);
            scenes2Bundle = scenes2Bundle.Where(i => i.Value != name).ToDictionary(i => i.Key, i => i.Value);
        }
    }
}

