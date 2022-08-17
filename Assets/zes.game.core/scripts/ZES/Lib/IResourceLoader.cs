using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Zes
{
    public interface IResourceLoader
    {
        Task<AssetBundle> LoadBundle(string name, Action<float> progress);
        Task<UnityEngine.Object> LoadAsset(string path, Type type);
    }
}
