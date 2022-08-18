using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Zes
{
    public class ResourceLoaderForRuntime : IResourceLoader
    {
        static Logger logger = Logger.GetLogger<ResourceLoaderForRuntime>();

        public Task<UnityEngine.Object> LoadAsset(AssetBundle bundle, string path, Type type)
        {
            throw new NotImplementedException();
        }

        public async Task<AssetBundle> LoadBundle(string name, Action<float> progress)
        {
            name = name.ToLower();
            logger.info($"begin to load bundle {name}");
            string bundlePath = Path.Combine(App.instance.persistentDataPath, name);
            if (!File.Exists(bundlePath))
            {
                bundlePath = Path.Combine(Application.streamingAssetsPath, name);
            }
            if (!File.Exists(bundlePath))
            {
                logger.error($"cannot find bundle in path: ({bundlePath})");
                return null;
            }
            var bundlereq = AssetBundle.LoadFromFileAsync(bundlePath);
            await Util.WaitAsyncOperation(bundlereq, progress);
            var bundle = bundlereq.assetBundle;
            if (bundle == null)
            {
                logger.error($"load bundle {name} failed, get null bundle");
                return null;
            }
            logger.info($"bundle {name} loaded");
            return bundle;
        }

        public Task<Scene> LoadScene(string name, bool additive, Action<float> progress)
        {
            throw new NotImplementedException();
        }

        public async Task<string> LoadText(string path)
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
                    logger.error($"{path} not found");
                    return "";
                }
                bytes = File.ReadAllBytes(path);
            }
            string str = Encoding.UTF8.GetString(bytes);
            return str;
        }
    }
}
