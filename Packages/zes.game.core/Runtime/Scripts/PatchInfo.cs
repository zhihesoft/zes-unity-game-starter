using System.IO;
using UnityEngine;

namespace Zes
{
    [System.Serializable]
    public class PatchInfo
    {
        public string version;
        public string url;
        public PatchFileInfo[] files;

        public string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        public static PatchInfo FromJson(string json)
        {
            return JsonUtility.FromJson<PatchInfo>(json);
        }

        public void Save(string path)
        {
            using (StreamWriter writer = new StreamWriter(path, false, Util.utf8WithoutBOM))
            {
                writer.Write(ToJson());
                writer.Close();
            }
        }
    }
}
