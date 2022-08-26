using System.IO;
using System.Text;
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
            return JsonUtility.ToJson(this);
        }

        public static PatchInfo FromJson(string json)
        {
            return JsonUtility.FromJson<PatchInfo>(json);
        }

        public void Save(string path)
        {
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.ASCII))
            {
                writer.Write(ToJson());
                writer.Close();
            }
        }
    }
}
