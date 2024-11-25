using UnityEngine;

namespace AccSaber.Utils
{
    public static class ColorUtils
    {
        public static Color ColorWithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}
