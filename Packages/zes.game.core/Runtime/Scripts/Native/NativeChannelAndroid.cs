using UnityEngine;

namespace Zes.Native
{
    public class NativeChannelAndroid : NativeChannel
    {
        const string channelClassName = "com.leadinvr.channel.NativeChannel";
        AndroidJavaClass javaClass;

        private void Awake()
        {
            javaClass = new AndroidJavaClass(channelClassName);
        }

        protected override void CallNativeMethod(NativeCallState state)
        {
            javaClass.CallStatic(
                "call",
                state.className,
                state.methodName,
                state.id,
                state.args);
        }
    }
}
