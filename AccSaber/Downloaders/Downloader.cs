using AccSaber.Utils;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using IPA.Utilities;
using SiraUtil.Logging;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Concurrent;

namespace AccSaber.Downloaders
{
    public abstract class Downloader
    {
        internal string USER_AGENT { get; set; }
        
        internal ConcurrentDictionary<string, UnityWebRequest> _ongoingWebRequests =
            new ConcurrentDictionary<string, UnityWebRequest>();

        private SiraLog _siraLog;

        public Downloader(SiraLog siraLog)
        {
            _siraLog = siraLog;
            USER_AGENT =
                $"Unity/{UnityEngine.Application.unityVersion} BeatSaber/{UnityGame.GameVersion} ModelDownloader/{Plugin.version}";
        }

        ~Downloader()
        {
            foreach (var (_, webRequest) in _ongoingWebRequests)
            {
                webRequest.Abort();
            }
        }

        public void CancelAllDownloads()
        {
            foreach (var (_, webRequest) in _ongoingWebRequests)
            {
                webRequest.Abort();
            }
        }

        internal async Task<T> MakeJsonRequestAsync<T>(string url, CancellationToken cancellationToken,
            Action<float> progressCallback = null)
        {
            var webRequest = await MakeRequestAsync(url, cancellationToken, progressCallback);

            if (webRequest == null)
            {
                return default(T);
            }

            try
            {
                T response = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                if (response == null)
                {
                    _siraLog.Info(webRequest.responseCode);
                }

                return response;
            }
            catch (Exception e)
            {
                _siraLog.Warn($"Error parsing response: {e.Message}");
                return default(T);
            }
        }

        internal async Task<Sprite> MakeImageRequestAsync(string url, CancellationToken cancellationToken,
            Action<float> progressCallback = null)
        {
            var webRequest = await MakeRequestAsync(url, cancellationToken, progressCallback);

            if (webRequest == null)
            {
                return null;
            }

            try
            {
                Sprite sprite = BeatSaberMarkupLanguage.Utilities.LoadSpriteRaw(webRequest.downloadHandler.data);
                return sprite;
            }
            catch (Exception e)
            {
                _siraLog.Warn($"Error parsing image: {e.Message}");
                return null;
            }
        }

        internal async Task<UnityWebRequest> MakeRequestAsync(string url, CancellationToken cancellationToken,
            Action<float> progressCallback = null)
        {
            var webRequest = UnityWebRequest.Get(url);
            webRequest.SetRequestHeader("User-Agent", USER_AGENT);
            webRequest.timeout = 15;

#if DEBUG
            _siraLog.Debug($"Making web request: {url}");
#endif
            _ongoingWebRequests.TryAdd(url, webRequest);

            webRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    webRequest.Abort();
                    throw new TaskCanceledException();
                }

                progressCallback?.Invoke(webRequest.downloadProgress);
                await Task.Yield();
            }

#if DEBUG
            _siraLog.Debug("Web request finished");
#endif
            _ongoingWebRequests.TryRemove(url, out _ );

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                _siraLog.Warn($"Error making request: {webRequest.error}");
                return null;
            }

            return webRequest;
        }
    }
}