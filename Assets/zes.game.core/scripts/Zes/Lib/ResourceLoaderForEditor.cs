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
    public class ResourceLoaderForEditor : MonoBehaviour, IResourceLoader
    {
        static Logger logger = Logger.GetLogger<ResourceLoaderForEditor>();

        public async Task<UnityEngine.Object> LoadAsset(string path, Type type)
        {
            var data = AssetDatabase.LoadAssetAtPath(path, type);
            await Task.Delay(0);
            return data;
        }

        public async Task<AssetBundle> LoadBundle(string name, Action<float> progress)
        {
            await Task.Delay(0);
            return null;
        }

        public async Task<Scene> LoadScene(string name, bool additive, Action<float> progress)
        {
            if (additive)
            {
                Scene loadedScene = default(Scene);
                UnityAction<Scene, LoadSceneMode> callback = (scene, mode) =>
                {
                    loadedScene = scene;
                };
                SceneManager.sceneLoaded += callback;

                var op = EditorSceneManager.LoadSceneAsyncInPlayMode(name, new LoadSceneParameters(LoadSceneMode.Additive));
                await Util.WaitAsyncOperation(op, progress);
                SceneManager.sceneLoaded -= callback;
                return loadedScene;
            }
            else
            {
                var op = SceneManager.LoadSceneAsync(name);
                await Util.WaitAsyncOperation(op, progress);
                return default(Scene);
            }
        }

        public async Task<string> LoadText(string path)
        {
            if (!File.Exists(path))
            {
                logger.error($"{path} not found");
                return "";
            }

            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                string s = reader.ReadToEnd();
                await Task.Delay(0);
                return s;
            }
        }
    }
}
