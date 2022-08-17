using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zes
{
    public interface IResourceLoader
    {
        /// <summary>
        /// Load Text in file system
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<string> LoadText(string path);
        /// <summary>
        /// Load bundle
        /// </summary>
        /// <param name="name"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task<AssetBundle> LoadBundle(string name, Action<float> progress);
        /// <summary>
        /// Load asset in bundle
        /// </summary>
        /// <param name="path">[bundle:]assetPath (bundle is optional, assetPath like 'Assets/test/test.prefab')</param>
        /// <param name="type">asset type</param>
        /// <returns></returns>
        Task<UnityEngine.Object> LoadAsset(string path, Type type);
        /// <summary>
        /// Load a scene
        /// </summary>
        /// <param name="name"></param>
        /// <param name="additive"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task<Scene> LoadScene(string name, bool additive, Action<float> progress);
    }
}
