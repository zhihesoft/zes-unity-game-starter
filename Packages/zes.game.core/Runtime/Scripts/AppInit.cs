using Puerts;
using UnityEngine;

namespace Zes
{
    public abstract class AppInit : MonoBehaviour
    {
        public abstract void BeforeInit();
        public abstract void OnInit(JsEnv env);
    }
}
