using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zes
{
    public abstract class ResourceLoader
    {
        public abstract Task<Scene> LoadScene(string name, bool additive, Action<float> progress);
        public abstract Task<AssetBundle> LoadBundle(string name, Action<float> progress = null);
        public abstract void UnloadBundle(AssetBundle bundle);
        public abstract Task<UnityEngine.Object> LoadAsset(AssetBundle bundle, string path, Type type);
        public abstract Task<string> LoadText(string path);
    }
}
