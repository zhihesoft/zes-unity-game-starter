#if USING_FIREBASE
using System.IO;
using UnityEditor.Android;
using UnityEngine;

namespace Zes
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
            string sourceGoogleJson = Path.Combine("google-services.json");
            string destGoogleJson = Path.Combine("Temp", "gradleOut", "launcher", "google-services.json");
            Debug.Log($"copy google-services.json from {sourceGoogleJson} to {destGoogleJson}");
            File.Copy(sourceGoogleJson, destGoogleJson);
        }
    }
}

#endif