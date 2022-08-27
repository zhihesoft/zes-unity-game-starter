using UnityEditor;
using Zes.Builders;

namespace Zes
{
    public class TestCommands
    {
        [MenuItem("ZES/Test/Compile TS")]
        public static void TestCompileTS()
        {
            var task = new BuildJavascript(EditorHelper.LoadAppConstants());
            task.Build();
        }

        [MenuItem("ZES/Test/Import Excels")]
        public static void TestImportExcels()
        {
            var task = new BuildConfigurations(EditorHelper.LoadAppConstants());
            task.Build();
        }


        [MenuItem("ZES/Test/Build Bundles")]
        public static void TestBuildBundles()
        {
            var task = new BuildBundle(EditorHelper.LoadAppConstants(), EditorUserBuildSettings.activeBuildTarget);
            task.Build();
        }


    }
}
