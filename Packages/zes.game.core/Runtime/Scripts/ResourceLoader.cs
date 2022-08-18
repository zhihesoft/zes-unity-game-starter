using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Zes
{
    public class ResourceLoader
    {

        public ResourceLoader()
        {
        }

#if UNITY_EDITOR
        private IResourceLoader loader = new ResourceLoaderForEditor();
#else
        private IResourceLoader loader = new ResourceLoaderForRuntime();
#endif

        private Dictionary<string, PendingItem<AssetBundle>> bundles = new Dictionary<string, PendingItem<AssetBundle>>();

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
        }

    }
}
