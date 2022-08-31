using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Zes.Native
{
    public abstract class NativeChannel : MonoBehaviour
    {
        private static Logger logger = Logger.GetLogger<NativeChannel>();

        public static NativeChannel Create()
        {
            Debug.Assert(GameObject.Find(nodeName) == null, "Native channel can only be init for once");

            GameObject go = new GameObject(nodeName);
            DontDestroyOnLoad(go);
            NativeChannel ret =
#if UNITY_ANDROID
                go.AddComponent<NativeChannelAndroid>();
#elif UNITY_IOS
                go.AddComponent<NativeChannelIOS>();
#else
                null;
#endif
            return ret;
        }

        private const string nodeName = "__native_channel__";
        private int callId = 0;
        private Dictionary<int, NativeCallState> states = new Dictionary<int, NativeCallState>();

        public Action<string, string> OnEvent;

        public async Task<NativeCallState> Call(string className, string methodName, string args = "")
        {
            var state = new NativeCallState(callId++, className, methodName, args);
            states.Add(state.id, state);
            CallNativeMethod(state);
            while (state.status == 0)
            {
                await Task.Yield();
            }
            return state;
        }


        public void OnNativeData(string message)
        {
            NativeResponse resp = JsonUtility.FromJson<NativeResponse>(message);
            if (resp.callId < 0)
            {
                OnEvent?.Invoke(resp.method, resp.message);
            }
            else
            {
                if (states.ContainsKey(resp.callId))
                {
                    var state = states[resp.callId];
                    states.Remove(resp.callId);
                    state.result = resp.message;
                    state.status = 1;
                    state.error = resp.error;
                }
                else
                {
                    logger.Error($"cannot find callid of ({resp.callId})");
                }
            }
        }

        protected abstract void CallNativeMethod(NativeCallState state);
    }
}
