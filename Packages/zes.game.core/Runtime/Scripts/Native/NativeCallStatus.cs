namespace Zes.Native
{
    public class NativeCallState
    {
        public readonly int id;
        public readonly string className;
        public readonly string methodName;
        public readonly string args;
        public string result;
        public int status; // 0 pending, 1 succ, -1 failed
        public int error = 0; // error code

        public NativeCallState(int id, string className, string methodName, string args)
        {
            this.id = id;
            this.className = className;
            this.methodName = methodName;
            this.args = args;
        }
    }
}
