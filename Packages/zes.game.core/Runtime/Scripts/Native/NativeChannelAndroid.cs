using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
