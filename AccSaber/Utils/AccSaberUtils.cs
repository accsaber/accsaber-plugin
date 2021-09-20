using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace AccSaber.Utils
{
    public static class AccSaberUtils
    {
        private static List<AccSaberCategory> knownCategories = new List<AccSaberCategory>();

        public class AccSaberAPISong
        {
            public string songName;
            public string songSubName;
            public string songAuthorName;
            public string levelAuthorName;
            public string difficulty;
            public string beatSaverKey;
            public string songHash;
            public float complexity;
            public string categoryDisplayName;
        }

        public class AccSaberSong
        {
            public string songName;
            public string songSubName;
            public string songAuthorName;
            public string levelAuthorName;
            public string beatSaverKey;
            public string songHash;
            public List<AccSaberSongDiff> diffs;
            [JsonIgnore]
            public bool downloaded = false;
            [JsonIgnore]
            public string artistSongNameString;
            [JsonIgnore]
            public string formattedName;
            [JsonIgnore]
            public string levelID;
            [JsonIgnore]
            public Sprite cover = null;

            public AccSaberSong(string inSongName, string inSongSubName, string inSongAuthorName, string inLevelAuthorName, string inBeatSaverKey, string inSongHash, List<AccSaberSongDiff> inDiffs)
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
                if (!downloaded)
                {
                    downloaded = SongCore.Collections.songWithHashPresent(songHash);
                    if (downloaded)
                    {
                        formattedName = "<color=#474949>" + artistSongNameString;
                    }
                }
                return downloaded;
            }
        }

        public class AccSaberSongDiff
        {
            public string categoryDisplayName;
            public string difficulty;
            public float complexity;
            [JsonIgnore]
            public string categoryComplexityString;

            public AccSaberSongDiff(string inCategoryDisplayName, string inDifficulty, float inComplexity)
            {
                categoryDisplayName = inCategoryDisplayName;
                difficulty = inDifficulty;
                complexity = inComplexity;

                categoryComplexityString = $"{categoryDisplayName} - {complexity.ToString("F1", CultureInfo.InvariantCulture)}";
            }
        }

        public class AccSaberCategory : ICloneable
        {
            public string categoryName;
            public string description;
            public string categoryDisplayName;
            public bool countsTowardsOverall;

            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        internal static void SetKnownCategory(AccSaberCategory category)
        {
            knownCategories.Add(category);
        }

        public static AccSaberCategory GetCategoryByDisplayName(string categoryDisplayName)
        {
            foreach (var category in knownCategories)
            {
                if (category.categoryDisplayName == categoryDisplayName)
                {
                    return (AccSaberCategory) category.Clone();
                }
            }

            return null;
        }

    }
}
