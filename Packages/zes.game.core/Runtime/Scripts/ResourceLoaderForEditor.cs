#if UNITY_EDITOR

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Zes
{
    public class ResourceLoaderForEditor : ResourceLoader
    {
        public override async Task<Scene> LoadScene(string name, bool additive, Action<float> progress)
        {
            Scene loadedScene = default(Scene);
            UnityAction<Scene, LoadSceneMode> callback = (scene, mode) =>
            {
                loadedScene = scene;
            };
            SceneManager.sceneLoaded += callback;

            var op = EditorSceneManager.LoadSceneAsyncInPlayMode(name,
                new LoadSceneParameters(additive ? LoadSceneMode.Additive : LoadSceneMode.Single));
            await Util.WaitAsyncOperation(op, progress);
            SceneManager.sceneLoaded -= callback;
            return loadedScene;
        }

        public override async Task<UnityEngine.Object> LoadAsset(AssetBundle bundle, string path, Type type)
        {
            var data = AssetDatabase.LoadAssetAtPath(path, type);
            await Task.Delay(0);
            return data;
        }

        public override async Task<AssetBundle> LoadBundle(string name, Action<float> progress)
        {
            await Task.Delay(0);
            progress?.Invoke(1);
            return null;
        }

        public override void UnloadBundle(AssetBundle bundle)
        {
        }
    }
}

#endif
