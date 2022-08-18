using UnityEngine;

namespace Zes
{
    [CreateAssetMenu(fileName = "app", menuName = "ZES/AppConfig", order = 1)]
    public class AppConfig : ScriptableObject
    {
        public string patchDataPath = "patch_data";
    }
}
