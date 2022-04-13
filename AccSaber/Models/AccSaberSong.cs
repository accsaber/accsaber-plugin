using System.Collections.Generic;
using AccSaber.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace AccSaber.Models
{
    public class AccSaberSong
    {
        public string songName;
        public string songSubName;
        public string songAuthorName;
        public string levelAuthorName;
        public string beatSaverKey;
        public string songHash;
        public List<AccSaberSongDiff> diffs;
        public bool downloaded = false;
        public string artistSongNameString;
        public string formattedName;
        public string levelID;
        public Sprite cover = null;

        public AccSaberSong(string inSongName, string inSongSubName, string inSongAuthorName, string inLevelAuthorName,
            string inBeatSaverKey, string inSongHash, List<AccSaberSongDiff> inDiffs)
        {
            songName = inSongName;
            songSubName = inSongSubName;
            songAuthorName = inSongAuthorName;
            levelAuthorName = inLevelAuthorName;
            beatSaverKey = inBeatSaverKey;
            songHash = inSongHash;
            diffs = inDiffs;

            artistSongNameString = $"{songAuthorName} - {songName}";
            downloaded = IsDownloaded();
            formattedName = $"{(IsDownloaded() ? "<color=#474949>" : "")}" + artistSongNameString;
            levelID = "custom_level_" + inSongHash.ToUpper();
        }

        public void AddDiff(AccSaberSongDiff diff)
        {
            diffs.Add(diff);
        }

        internal bool IsDownloaded()
        {
            bool oldStatus = downloaded;
            downloaded = SongCore.Collections.songWithHashPresent(songHash);
            if (oldStatus != downloaded)
            {
                formattedName = (downloaded ? "<color=#474949>" : "") + artistSongNameString;
            }

            return downloaded;
        }
    }
}