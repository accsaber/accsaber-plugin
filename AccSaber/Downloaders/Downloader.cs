using AccSaber.Utils;
using Newtonsoft.Json;
using SiraUtil.Tools;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AccSaber.Downloaders
{
    public abstract class Downloader
    {
        internal string USER_AGENT { get; set; }

        internal ConcurrentHashSet<UnityWebRequest> _ongoingWebRequests = new ConcurrentHashSet<UnityWebRequest>();

        private SiraLog _siraLog;

        public Downloader(SiraLog siraLog)
        {
            _siraLog = siraLog;
            USER_AGENT = $"AccSaberPlugin/v{Plugin.version}";
        }

        ~Downloader()
        {
            foreach (var webRequest in _ongoingWebRequests)
            {
                webRequest.Abort();
            }
        }

        public void CancelAllDownloads()
        {
            foreach (var webRequest in _ongoingWebRequests)
            {
                webRequest.Abort();
            }
        }

        internal async Task<T> MakeJsonRequestAsync<T>(string url, CancellationToken cancellationToken, Action<float> progressCallback = null)
        {
            var www = await MakeRequestAsync(url, cancellationToken, progressCallback);

            if (www == null)
            {
                return default(T);
            }

            try
            {
                T response = JsonConvert.DeserializeObject<T>(www.downloadHandler.text);

                return response;
            }
            catch (Exception e)
            {
                _siraLog.Warning($"Error parsing response: {e.Message}");
                return default(T);
            }
        }

        internal async Task<Sprite> MakeImageRequestAsync(string url, CancellationToken cancellationToken, Action<float> progressCallback = null)
        {
            var www = await MakeRequestAsync(url, cancellationToken, progressCallback);

            if (www == null)
            {
                return null;
            }

            try
            {
                Sprite sprite = BeatSaberMarkupLanguage.Utilities.LoadSpriteRaw(www.downloadHandler.data);
                return sprite;
            }
            catch (Exception e)
            {
                _siraLog.Warning($"Error parsing image: {e.Message}");
                return null;
            }
        }

        internal async Task<UnityWebRequest> MakeRequestAsync(string url, CancellationToken cancellationToken, Action<float> progressCallback = null)
        {
            var www = UnityWebRequest.Get(url);
            www.SetRequestHeader("User-Agent", USER_AGENT);
            www.timeout = 15;

#if DEBUG
            _siraLog.Debug($"Making web request: {url}");
#endif
            _ongoingWebRequests.Add(www);

            www.SendWebRequest();

            while (!www.isDone)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    www.Abort();
                    throw new TaskCanceledException();
                }
                progressCallback?.Invoke(www.downloadProgress);
                await Task.Yield();
            }

#if DEBUG
            _siraLog.Debug("Web request finished");
#endif
            _ongoingWebRequests.Remove(www);

            if (www.isNetworkError || www.isHttpError)
            {
                _siraLog.Warning($"Error making request: {www.error}");
                return null;
            }

            return www;
        }
    }
}
