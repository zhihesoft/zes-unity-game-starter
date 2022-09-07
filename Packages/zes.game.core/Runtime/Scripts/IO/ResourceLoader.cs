using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Zes.IO
{
    public abstract class ResourceLoader
    {
        public static ResourceLoader GetLoader()
        {
#if UNITY_EDITOR
            var loader = new ResourceLoaderForEditor();
#elif USING_AAB
            var loader = new ResourceLoaderForAAB();
#else
            var loader = new ResourceLoaderForRuntime();
#endif
            return loader;
        }

        protected Logger logger = Logger.GetLogger<ResourceLoader>();

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
                    logger.Error($"{path} not found");
                    return "";
                }
                bytes = File.ReadAllBytes(path);
            }
            string str = Encoding.UTF8.GetString(bytes);
            return str;
        }

        public abstract Task<Scene> LoadScene(string name, bool additive, Action<float> progress);
        public abstract Task<AssetBundle> LoadBundle(string name, Action<float> progress = null);
        public abstract void UnloadBundle(AssetBundle bundle);
        public abstract Task<bool> UnloadScene(Scene scene);
        public abstract Task<UnityEngine.Object> LoadAsset(AssetBundle bundle, string path, Type type);

    }
}
