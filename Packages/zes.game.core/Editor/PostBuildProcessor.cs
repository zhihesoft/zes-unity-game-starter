#if USING_FIREBASE
using System.IO;
using UnityEditor.Android;
using UnityEngine;

namespace ZEditor
{
    public class PostBuildProcessor : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder
        {
            get
            {
                return 999;
            }
        }

        void IPostGenerateGradleAndroidProject.OnPostGenerateGradleAndroidProject(string path)
        {
            Debug.Log("Bulid path : " + path);
            // copy google-services.json to build path
            string sourceGoogleJson = Path.Combine("Assets", "Plugins", "Android", "google-services.json");
            // string destGoogleJson = Path.Combine(path, "google-services.json");
            string destGoogleJson = Path.Combine("Temp", "gradleOut", "launcher", "google-services.json");
            Debug.Log($"copy google-services.json from {sourceGoogleJson} to {destGoogleJson}");
            File.Copy(sourceGoogleJson, destGoogleJson);
        }
    }
}

#endif