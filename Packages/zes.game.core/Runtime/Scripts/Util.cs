using System;
using System.Text;
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

        /// <summary>
        /// get unix timestamp (seconds from 1970-1-1)
        /// </summary>
        /// <returns></returns>
        public static long Timestamp()
        {
            var offset = new DateTimeOffset(DateTime.UtcNow);
            long stamp = offset.ToUnixTimeSeconds();
            return stamp;
        }

        /// <summary>
        /// parse an asset path like: bundle:Assets/xxx/yyy to [bundle, path] formation
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] ParseAssetPath(string path)
        {
            var parts = path.Split(':');
            if (parts.Length < 2)
            {
                return parts;
            }
            return new string[] { parts[0].Trim(), parts[1].Trim() };
        }

        public static Encoding Utf8WithoutBOM()
        {
            return new UTF8Encoding(false);
        }
    }
}
