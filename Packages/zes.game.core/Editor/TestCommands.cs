using UnityEditor;
using Zes.Builders;

namespace Zes
{
    public class TestCommands
    {
        [MenuItem("ZES/Test/Compile TS")]
        public static void TestCompileTS()
        {
            BuildRunner.Run(EditorUserBuildSettings.activeBuildTarget, new BuildJavascript());
        }

        [MenuItem("ZES/Test/Import Excels")]
        public static void TestImportExcels()
        {
            BuildRunner.Run(EditorUserBuildSettings.activeBuildTarget, new BuildConfigurations());
        }


        [MenuItem("ZES/Test/Build Bundles")]
        public static void TestBuildBundles()
        {
            BuildRunner.Run(EditorUserBuildSettings.activeBuildTarget, new BuildBundle());
        }

        [MenuItem("ZES/Test/Build App")]
        public static void TestBuildApp()
        {
            BuildRunner.Run(EditorUserBuildSettings.activeBuildTarget, new BuildApp());
        }


    }
}
