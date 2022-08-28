using System.IO;
using UnityEngine;

namespace Zes
{
    [System.Serializable]
    public class PlatformConfig
    {
        // 配置名称
        public string name;
        // Android 证书密码
        public string androidKeystorePassword;
        // Android 签名密码
        public string androidKeyAliasPassword;
        // dependences (will install by openupm)
        public string[] dependencies;

        public static PlatformConfig Load()
        {
            using (StreamReader fs = new StreamReader("platform.json", Util.Utf8WithoutBOM()))
            {
                string json = fs.ReadToEnd();
                return JsonUtility.FromJson<PlatformConfig>(json);
            }
        }
    }
}
