using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Zes
{
    public static class Util
    {
        // get no bom utf8 encoding
        public static Encoding utf8WithoutBOM = new UTF8Encoding(false);

        public static async Task WaitAsyncOperation(AsyncOperation op, Action<float> progress = null)
        {
            while (!op.isDone)
            {
                progress?.Invoke(op.progress);
                await Task.Yield();
            }
            progress?.Invoke(1);
        }

        public static async Task WaitUntil(Func<bool> condition)
        {
            while (!condition())
            {
                await Task.Yield();
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
        /// Copy file
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static async Task<bool> FileCopy(string source, string dest)
        {
            if (source.StartsWith("jar:"))
            {
                var www = UnityWebRequest.Get(source);
                www.downloadHandler = new DownloadHandlerFile(dest);
                var op = www.SendWebRequest();
                await WaitAsyncOperation(op);
            }
            else
            {
                File.Copy(source, dest, true);
            }
            return true;
        }

        public static void FileSave(string text, string dest)
        {
            File.WriteAllText(dest, text);
        }

        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static DirectoryInfo DirEnsure(string dir)
        {
            return DirEnsure(new DirectoryInfo(dir));
        }

        // ensure dir exist
        public static DirectoryInfo DirEnsure(DirectoryInfo dir)
        {
            if (!dir.Parent.Exists)
            {
                DirEnsure(dir.Parent);
            }

            if (!dir.Exists)
            {
                dir.Create();
            }

            return dir;
        }

        public static DirectoryInfo DirClear(string dir)
        {
            return DirClear(new DirectoryInfo(dir));
        }

        // clear dir
        public static DirectoryInfo DirClear(DirectoryInfo dir)
        {
            DirEnsure(dir);
            dir.GetFiles().ToList().ForEach(f => f.Delete());
            dir.GetDirectories().ToList().ForEach(d => d.Delete(true));
            return dir;
        }

        public static void DirCopy(string from, string to)
        {
            DirCopy(new DirectoryInfo(from), new DirectoryInfo(to));
        }

        public static void DirCopy(DirectoryInfo from, DirectoryInfo to)
        {
            if (!from.Exists)
            {
                Debug.LogError($"Copy dir failed: {from.FullName} not existed");
                return;
            }

            DirEnsure(to);

            from.GetFiles().ToList().ForEach(file => file.CopyTo(Path.Combine(to.FullName, file.Name), true));
            from.GetDirectories().ToList().ForEach(dir => DirCopy(dir, new DirectoryInfo(Path.Combine(to.FullName, dir.Name))));
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
