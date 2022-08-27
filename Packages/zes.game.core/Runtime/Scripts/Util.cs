using System;
using System.IO;
using System.Linq;
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

        // get no bom utf8 encoding
        public static Encoding Utf8WithoutBOM()
        {
            return new UTF8Encoding(false);
        }

        public static DirectoryInfo EnsureDir(string dir)
        {
            return EnsureDir(new DirectoryInfo(dir));
        }

        // ensure dir exist
        public static DirectoryInfo EnsureDir(DirectoryInfo dir)
        {
            if (!dir.Parent.Exists)
            {
                EnsureDir(dir.Parent);
            }

            if (!dir.Exists)
            {
                dir.Create();
            }

            return dir;
        }

        public static DirectoryInfo ClearDir(string dir)
        {
            return ClearDir(new DirectoryInfo(dir));
        }

        // clear dir
        public static DirectoryInfo ClearDir(DirectoryInfo dir)
        {
            EnsureDir(dir);
            dir.GetFiles().ToList().ForEach(f => f.Delete());
            dir.GetDirectories().ToList().ForEach(d => d.Delete(true));
            return dir;
        }

        public static void CopyDir(string from, string to)
        {
            CopyDir(new DirectoryInfo(from), new DirectoryInfo(to));
        }

        public static void CopyDir(DirectoryInfo from, DirectoryInfo to)
        {
            if (!from.Exists)
            {
                Debug.LogError($"Copy dir failed: {from.FullName} not existed");
                return;
            }

            EnsureDir(to);

            from.GetFiles().ToList().ForEach(file => file.CopyTo(Path.Combine(to.FullName, file.Name), true));
            from.GetDirectories().ToList().ForEach(dir => CopyDir(dir, new DirectoryInfo(Path.Combine(to.FullName, dir.Name))));
        }

        public static string CombineUri(string baseUri, string path)
        {
            if (!baseUri.EndsWith("/"))
            {
                baseUri += "/";
            }

            return new Uri(new Uri(baseUri), path).ToString();
        }
    }
}
