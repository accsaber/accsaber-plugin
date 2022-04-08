using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using AccSaber.Models;
using UnityEngine;

namespace AccSaber.Utils
{
    public static class AccSaberUtils
    {
        private static List<AccSaberCategory> knownCategories = new List<AccSaberCategory>();

        public static Dictionary<string, Color> CategoryColors = new Dictionary<string, Color>()
        {
            { "True Acc", new Color(0.015f, 0.906f, 0.176f, 1) },
            { "Standard Acc", new Color(0.039f, 0.573f, 0.918f, 1) },
            { "Tech Acc", new Color(0.902f, 0.027f, 0.027f, 1) }
        };

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
                    return (AccSaberCategory)category.Clone();
                }
            }

            return null;
        }
    }
}
