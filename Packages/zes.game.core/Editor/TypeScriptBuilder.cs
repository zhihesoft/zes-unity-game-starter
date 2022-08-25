using UnityEditor;

namespace Zes
{
    public class TypeScriptBuilder
    {
        [MenuItem("ZES/Test/Compile TS")]
        public static void TestCompileTS()
        {
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = "jsbin";
            buildMap[0].assetNames = new string[] {
                "Assets/jsbin/main.bytes"
            };
            BuildPipeline.BuildAssetBundles("AssetBundles", buildMap, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }


    }
}
