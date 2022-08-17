using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ZEditor
{
    public class PlatformConfig
    {
        // 配置名称
        public string name;
        // 开发商
        public string company;
        // 产品名称
        public string productName;
        // ApplicationID
        public string applicationId;
        // 证书文件
        public string keystore;
        // 证书密码
        public string keystorePassword;
        // 签名
        public string keyAlias;
        // 签名密码
        public string keyAliasPassword;
        // Android SDK 版本
        public AndroidSdkVersions targetSdkVersion;
        // 描述
        public string description;

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
