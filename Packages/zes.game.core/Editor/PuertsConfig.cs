using Puerts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using Zes.IO;
using Zes.Native;

namespace Zes
{
    [Configure]
    public class PuertsConfig
    {
        [Binding]
        public static IEnumerable<Type> Bindings
        {
            get
            {
                return new List<Type>() {
                    typeof(Application),
                    typeof(SystemInfo),
                    typeof(Array),
                    typeof(Vector2),
                    typeof(Vector3),
                    typeof(AsyncOperation),
                    typeof(Resources),
                    typeof(TextAsset),
                    typeof(UnityEngine.Object),
                    typeof(GameObject),
                    typeof(Transform),
                    typeof(Component),
                    typeof(AssetBundle),
                    typeof(Sprite),
                    typeof(Color),
                    typeof(Scene),
                    typeof(LoadSceneParameters),
                    typeof(LoadSceneMode),
                    typeof(UnityEngine.UI.Graphic),
                    typeof(UnityEngine.UI.Image),
                    typeof(UnityEngine.UI.Button),
                    typeof(UnityEngine.UI.Toggle),
                    typeof(UnityEngine.UI.ToggleGroup),
                    typeof(UnityEngine.UI.Slider),
                    typeof(UnityEngine.UI.Button.ButtonClickedEvent),
                    typeof(UnityEngine.Events.UnityEvent),
                    typeof(UnityEngine.Events.UnityEventBase),
                    typeof(SceneManager),
                    typeof(PlayerPrefs),
                    typeof(VideoPlayer),
                    typeof(VideoClip),
                    typeof(Behaviour),
                    typeof(Animator),
                    typeof(UnityEngine.AI.NavMeshAgent),

                    typeof(TMPro.TMP_Text),

                    typeof(App),
                    typeof(AppConfig),
                    typeof(ResourceLoader),
                    typeof(WSConnector),
                    typeof(HttpConnector),
                    typeof(HttpResult),
                    typeof(NativeChannel),
                    typeof(NativeCallState),
                    typeof(NativeResponse),
                    typeof(Tags),
                    typeof(Util),
                };
            }
        }

        static Dictionary<Type, Dictionary<string, bool>> filters = new Dictionary<Type, Dictionary<string, bool>>
        {
            { typeof(UnityEngine.UI.Graphic), new Dictionary<string, bool>{ { "OnRebuildRequested", true } } },
        };

        [Filter]
        public static bool Filter(System.Reflection.MemberInfo memberInfo)
        {
            if (filters.TryGetValue(memberInfo.DeclaringType, out var methods))
            {
                return methods.ContainsKey(memberInfo.Name);
            }
            else
            {
                return false;
            }
        }
    }
}

