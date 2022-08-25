using System.IO;
using UnityEngine;

namespace Zes
{
    [System.Serializable]
    public class PlatformConfig
    {
        // 配置名称
        public string name;
        // 描述
        public string description;
        // Android 证书密码
        public string androidKeystorePassword;
        // Android 签名密码
        public string androidKeyAliasPassword;

        public static PlatformConfig load()
        {
            using (StreamReader fs = new StreamReader("platform.json", Util.Utf8WithoutBOM()))
            {
                string json = fs.ReadToEnd();
                return JsonUtility.FromJson<PlatformConfig>(json);
            }
        }
    }
}
