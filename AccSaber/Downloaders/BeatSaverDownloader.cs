using SiraUtil.Tools;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using static AccSaber.Utils.AccSaberUtils;
using static AccSaber.Utils.BeatSaverUtils;

namespace AccSaber.Downloaders
{
    public class BeatSaverDownloader : Downloader
    {
        private const string API_URL = "https://api.beatsaver.com/";
        private const string HASH_ENDPOINT = "maps/hash/";

        private const string KNOWN_HASH = "AD6C9F88D63259A95E39397C31BE2981C4BEB744";
        // I hope we never have to use this 😔
        private const string BACKUP_HASH = "CB9F1581FF6C09130C991DB8823C5953C660688F";

        private const string TEMPLATE_STRING = "<hash>";

        private static string _cdnURL = null;

        private readonly SiraLog _siraLog;

        public BeatSaverDownloader(SiraLog siraLog) : base(siraLog)
        {
            _siraLog = siraLog;
        }

        /// <returns>True iff the song could be successfully downloaded</returns>
        public async Task<bool> DownloadOldVersionByHash(string hash, CancellationToken cancellationToken, AccSaberSong accSaberSong = null, Action<float> progressCallback = null, Action<string> statusCallback = null)
        {
            if (_cdnURL == null)
            {
                // haven't found template yet, make it
                try
                {
                    statusCallback?.Invoke("Finding cdn...");
                    string knownURL = API_URL + HASH_ENDPOINT + KNOWN_HASH;
                    BeatSaverAPISong apiSong = await MakeJsonRequestAsync<BeatSaverAPISong>(knownURL, cancellationToken);
                    if (apiSong.versions.Count == 0)
                    {
                        // backup time
                        knownURL = API_URL + HASH_ENDPOINT + BACKUP_HASH;
                        apiSong = await MakeJsonRequestAsync<BeatSaverAPISong>(knownURL, cancellationToken);
                        if (apiSong.versions.Count == 0)
                        {
                            _siraLog.Debug("Could not get download link for either known hash");
                            return false;
                        }
                    }

                    var apiVersion = apiSong.versions[0];
                    if (!apiVersion.downloadURL.Contains(apiVersion.hash))
                    {
                        _siraLog.Debug("Failed to convert download url into template");
                        return false;
                    }
                    _cdnURL = apiVersion.downloadURL.Replace(apiVersion.hash, TEMPLATE_STRING);
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            statusCallback?.Invoke("Downloading song...");
            string url = _cdnURL.Replace(TEMPLATE_STRING, hash.ToLower());
            var www = await MakeRequestAsync(url, cancellationToken, progressCallback);

            if (www == null)
            {
                return false;
            }

            try
            {
                string path = CustomLevelPathHelper.customLevelsDirectoryPath;
                if (accSaberSong == null)
                {
                    path = Path.Combine(path, hash.ToLower());
                }
                else
                {
                    path = Path.Combine(CustomLevelPathHelper.customLevelsDirectoryPath, $"{accSaberSong.beatSaverKey} ({accSaberSong.songName} - {accSaberSong.levelAuthorName})");
                    int c = 0;

                    // if this is inefficient then you need to stop downloading the same song
                    while (Directory.Exists(path))
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            throw new TaskCanceledException();
                        }

                        if (c == 0)
                        {
                            path += $" ({c + 1})";
                        }
                        else
                        {
                            path.Replace($"({c})", $"({c + 1})");
                        }
                        c++; // 😊
                    }
                }

                // cancel just before extracting - don't cancel if mid-extraction
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }
                Stream zipStream = new MemoryStream(www.downloadHandler.data);

                statusCallback?.Invoke("Extracting song...");
                await ExtractZipAsync(zipStream, path);
                return true;
            }
            catch (Exception e)
            {
                _siraLog.Warning($"Error extracting song: {e.Message}");
                return false;
            }
        }

        private async Task ExtractZipAsync(Stream zipStream, string path)
        {
            // throw error back up

            ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
            await Task.Run(() => archive.ExtractToDirectory(path)).ConfigureAwait(false);
            archive.Dispose();
            zipStream.Close();
        }
    }
}
