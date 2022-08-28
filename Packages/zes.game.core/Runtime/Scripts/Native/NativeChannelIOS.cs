using System.Runtime.InteropServices;

namespace Zes.Native
{
    public class NativeChannelIOS : NativeChannel
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        // 给iOS传int参数,无返回值,返回值通过iOS的return方法返回给Unity
        private static extern void nativeBridgeEntry(int callId, string name, string args);
#endif

        protected override void CallNativeMethod(NativeCallState state)
        {
#if UNITY_IOS
            string args = state.args == null ? "" : string.Join(",", state.args);
            nativeBridgeEntry(state.id, state.methodName, args);
#endif
        }
    }
}
