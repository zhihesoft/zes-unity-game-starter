using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Zes
{
    public static class Util
    {
        public static async Task WaitAsyncOperation(AsyncOperation op, Action<float> progress = null)
        {
            while (!op.isDone)
            {
                progress?.Invoke(op.progress);
                await Task.Delay(0);
            }
            progress?.Invoke(1);
        }

        public static async Task WaitUntil(Func<bool> condition)
        {
            while (!condition())
            {
                await Task.Delay(0);
            }
        }
    }
}
