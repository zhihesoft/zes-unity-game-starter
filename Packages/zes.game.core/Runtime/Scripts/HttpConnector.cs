using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Zes
{
    public class HttpConnector
    {
        private Logger logger = Logger.GetLogger<HttpConnector>();

        public HttpConnector(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public readonly string baseUrl;
        private string token = null;

        public void SetToken(string token)
        {
            this.token = token;
        }

        public async Task<HttpResult> Get(string url)
        {
            using (var www = UnityWebRequest.Get(url))
            {
                www.downloadHandler = new DownloadHandlerBuffer();
                SetHeads(www);
                var op = www.SendWebRequest();
                await Util.WaitAsyncOperation(op);
                if (www.result != UnityWebRequest.Result.Success)
                {
                    logger.Error($"Get {www.url} failed: \n({www.responseCode}) {www.error})");
                    int code = (int)(www.responseCode == 0 ? 500 : www.responseCode);
                    return new HttpResult(code, www.error);
                }
                string str = www.downloadHandler.text;
                return new HttpResult(str);
            }
        }

        public async Task<HttpResult> Post(string url, string json)
        {
            using (var www = new UnityWebRequest(url, "POST"))
            {
                www.downloadHandler = new DownloadHandlerBuffer();
                if (!string.IsNullOrEmpty(json))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(json);
                    www.uploadHandler = new UploadHandlerRaw(bytes);
                    www.SetRequestHeader("Content-Type", "application/json");
                }
                SetHeads(www);
                var op = www.SendWebRequest();
                await Util.WaitAsyncOperation(op);
                if (www.result != UnityWebRequest.Result.Success)
                {
                    logger.Error($"Post {www.url} failed: \n({www.responseCode}) {www.error})");
                    int code = (int)(www.responseCode == 0 ? 500 : www.responseCode);
                    return new HttpResult(code, www.error);
                }

                string str = www.downloadHandler.text;
                return new HttpResult(str);
            }
        }

        public async Task<bool> Download(string url, string targetPath, Action<float> progress)
        {
            using (var www = UnityWebRequest.Get(url))
            {
                www.downloadHandler = new DownloadHandlerFile(targetPath);
                var webreq = www.SendWebRequest();
                while (!webreq.isDone)
                {
                    progress?.Invoke(webreq.progress);
                    await Task.Yield();
                }
                progress?.Invoke(1);

                if (www.result != UnityWebRequest.Result.Success)
                {
                    logger.Error($"Download {www.url} failed: \n({www.responseCode}) {www.error})");
                    return false;
                }

                return www.result == UnityWebRequest.Result.Success;
            }

        }

        private void SetHeads(UnityWebRequest www)
        {
            if (!string.IsNullOrEmpty(token))
            {
                www.SetRequestHeader("Authorization", $"Bearer {token}");
            }
        }

    }
}
