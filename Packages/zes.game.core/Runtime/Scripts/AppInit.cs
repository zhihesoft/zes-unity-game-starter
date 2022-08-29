using Puerts;
using UnityEngine;

namespace Zes
{
    public abstract class AppInit : MonoBehaviour
    {
        public virtual void OnStart() { }
        public abstract void OnInit(JsEnv env);
    }
}
