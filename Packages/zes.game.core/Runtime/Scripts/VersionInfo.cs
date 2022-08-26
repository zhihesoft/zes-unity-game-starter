using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Zes
{
    [Serializable]
    public class VersionInfo : IComparable<VersionInfo>
    {
        public string version;
        public string url;
        public string minVersion;

        /// <summary>
        /// Compare two version
        /// if this<other return -1
        /// if this=other return 0
        /// if this>other return 1
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(VersionInfo other)
        {
            string version1 = version;
            string version2 = other.version;

            if (string.IsNullOrEmpty(version1) || string.IsNullOrEmpty(version2))
            {
                return 1;
            }

            string[] v1 = version1.Split('.');
            string[] v2 = version2.Split('.');
            for (int i = 0; i < v1.Length; i++)
            {
                if (i >= v2.Length)
                {
                    return 1;
                }

                int.TryParse(v1[i], out int a);
                int.TryParse(v2[i], out int b);
                if (a != b)
                {
                    return a - b;
                }
            }
            if (v2.Length > v1.Length)
            {
                return -1;
            }
            return 0;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static VersionInfo FromJson(string json)
        {
            return JsonUtility.FromJson<VersionInfo>(json);
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
