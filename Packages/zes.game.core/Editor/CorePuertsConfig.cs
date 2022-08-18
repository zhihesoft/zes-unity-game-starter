using Puerts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

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

                    typeof(App),
                };
            }
        }

        static Dictionary<Type, string[]> filters = new Dictionary<Type, string[]>
        {
            { typeof(UnityEngine.UI.Graphic), new string []{ "OnRebuildRequested" } }
        };

        [Filter]
        public static bool Filter(System.Reflection.MemberInfo memberInfo)
        {
            if (filters.TryGetValue(memberInfo.DeclaringType, out var methods))
            {
                bool ret = methods.ToList().Contains(memberInfo.Name);
                // Debug.Log($"type {memberInfo.Name} check on {memberInfo.DeclaringType.FullName} ret is {ret}");
                return ret;
            }
            else
            {
                return false;
            }
        }
    }
}

