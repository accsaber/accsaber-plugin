using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;
using static AccSaber.Utils.AccSaberUtils;

namespace AccSaber.Models
{
    public class AccSaberSongDiff
    {
        public string categoryDisplayName;
        public string difficulty;
        public float complexity;
        public string categoryComplexityString;

        public AccSaberSongDiff(string inCategoryDisplayName, string inDifficulty, float inComplexity)
        {
            categoryDisplayName = inCategoryDisplayName;
            difficulty = inDifficulty;
            complexity = inComplexity;

            categoryComplexityString =
                $"<color=#{GetCategoryColor(categoryDisplayName)}>{categoryDisplayName} - {complexity.ToString("F1", CultureInfo.InvariantCulture)}";
        }

        private string GetCategoryColor(string categoryDisplayName)
        {
            if (CategoryColors.ContainsKey(categoryDisplayName))
            {
                return ColorUtility.ToHtmlStringRGB(CategoryColors[categoryDisplayName]);
            }

            return "FFFFFF";
        }
    }
}