using Google.Play.AssetDelivery;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Zes
{
    public class ResourceLoaderForAAB : ResourceLoaderForRuntime
    {
        public override async Task<AssetBundle> LoadBundle(string name, Action<float> progress = null)
        {
            logger.Info($"AAB loader loading bundle ({name})");
            string bundleDataPath = Path.Combine(App.persistentDataPath, name);
            if (!File.Exists(bundleDataPath))
            {
                logger.Info($"{bundleDataPath} not found, load from aab");
                PlayAssetBundleRequest bundleReq = PlayAssetDelivery.RetrieveAssetBundleAsync(name);
                await Util.WaitUntil(() => !bundleReq.keepWaiting);
                if (bundleReq.Status != AssetDeliveryStatus.Loaded)
                {
                    logger.Error($"cannot load bundle ({name}) from aab");
                    return null;
                }
                return bundleReq.AssetBundle;
            }
            else
            {
                var ret = await base.LoadBundle(name, progress);
                return ret;
            }
        }

    }
}
