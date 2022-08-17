using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Zes
{
    public class ResourceLoaderForRuntime : MonoBehaviour, IResourceLoader
    {
        public Task<UnityEngine.Object> LoadAsset(string path, Type type)
        {
            throw new NotImplementedException();
        }

        public Task<AssetBundle> LoadBundle(string name, Action<float> progress)
        {
            throw new NotImplementedException();
        }
    }
}
