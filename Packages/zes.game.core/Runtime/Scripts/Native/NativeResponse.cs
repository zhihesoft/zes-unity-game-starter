using System;

namespace Zes.Native
{
    [Serializable]
    public class NativeResponse
    {
        public int callId;
        public string method;
        public int error;
        public string message;
    }
}
