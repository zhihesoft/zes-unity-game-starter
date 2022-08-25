using System.IO;
using System.Text;
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
        // Typescript project path, relative to project root dir
        public string typescriptProjectPath = "Typescripts";
        // Android 证书密码
        public string androidKeystorePassword;
        // Android 签名密码
        public string androidKeyAliasPassword;
        // Android if aab format enabled
        public bool enableAAB;

        public static PlatformConfig load()
        {
            using (StreamReader fs = new StreamReader("platform.json", Encoding.UTF8))
            {
                string json = fs.ReadToEnd();
                return JsonUtility.FromJson<PlatformConfig>(json);
            }
        }
    }
}
