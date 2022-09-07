namespace Zes
{
    [System.Serializable]
    public class PlatformConfig
    {
        // 配置名称
        public string name;

        // language id start from
        public int languageStartId = 18000;
        // language config name
        public string languageConfigName = "language";
        // bundle output path
        public string bundleOutputPath = "AssetBundles";
        // javascript project path
        public string javascriptProjectPath = "Typescripts";
        // javascript entry for editor
        public string javascriptBuildResult = "./out/main.bytes";
        // Android 证书密码
        public string androidKeystorePassword;
        // Android 签名密码
        public string androidKeyAliasPassword;
        // dependences (will install by openupm)
        public string[] dependencies;
    }
}
