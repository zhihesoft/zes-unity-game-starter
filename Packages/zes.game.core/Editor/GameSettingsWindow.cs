// using Liv;
using UnityEditor;
using UnityEngine;
using Zes.Settings;

namespace Zes
{
    public class GameSettingsWindow : EditorWindow
    {

        [MenuItem("ZES/Settings", priority = 1)]
        public static void ShowGameSettingsWindow()
        {
            var window = GetWindow<GameSettingsWindow>("游戏设置");
            window.Show();
            window.minSize = new Vector2(800, 600);
            window.Initialize();
        }

        private SettingsManager manager;

        void Initialize()
        {
            if (manager == null)
            {
                manager = new SettingsManager();
                manager.Initialize();
            }
        }

        private void OnGUI()
        {
            if (manager == null)
            {
                Initialize();
            }
            manager?.OnGUI();
        }
    }
}
