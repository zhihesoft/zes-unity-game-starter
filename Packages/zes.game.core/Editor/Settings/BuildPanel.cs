using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zes.Builders;

namespace Zes.Settings
{
    public class BuildPanel : SettingPanel
    {
        private Logger logger = Logger.GetLogger<BuildPanel>();
        public override string Name => "Build";

        public override string DisplayName => "Build";

        public override string Description => "Build project";

        private int buildNo = 0;

        public override void OnShow()
        {
            base.OnShow();
            buildNo = BuildNo.Get();
        }

        public override void OnGUI()
        {
            using (new GUIIndent())
            {
                BuildConfigurations.excelsPath = EditorGUILayout.TextField("Excels", BuildConfigurations.excelsPath);
                BuildApp.outputDir = EditorGUILayout.TextField("Output Dir", BuildApp.outputDir);
                EditorGUILayout.LabelField("Build no.", buildNo.ToString());

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Build Bundles", GUILayout.Width(128), GUILayout.Height(32)))
                    {
                        var constants = EditorHelper.LoadAppConstants();
                        var tasks = new List<BuildTask>()
                        {
                            new BuildJavascript(constants, EditorUserBuildSettings.activeBuildTarget),
                            new BuildConfigurations(constants, EditorUserBuildSettings.activeBuildTarget),
                            new BuildBundle(constants, EditorUserBuildSettings.activeBuildTarget),
                        };
                        foreach (var task in tasks)
                        {
                            if (!task.Build())
                            {
                                logger.Error($"Abort: task {task.name} failed");
                                return;
                            }
                        }

                        buildNo = BuildNo.Inc();
                    }
                    if (GUILayout.Button("Build", GUILayout.Width(64), GUILayout.Height(32)))
                    {
                        var constants = EditorHelper.LoadAppConstants();
                        var tasks = new List<BuildTask>()
                        {
                            new BuildJavascript(constants, EditorUserBuildSettings.activeBuildTarget),
                            new BuildConfigurations(constants, EditorUserBuildSettings.activeBuildTarget),
                            new BuildBundle(constants, EditorUserBuildSettings.activeBuildTarget),
                            new BuildApp(constants, EditorUserBuildSettings.activeBuildTarget),
                        };
                        foreach (var task in tasks)
                        {
                            if (!task.Build())
                            {
                                logger.Error($"Abort: task {task.name} failed");
                                return;
                            }
                        }

                        buildNo = BuildNo.Inc();
                    }
                }
            }
        }
    }
}
