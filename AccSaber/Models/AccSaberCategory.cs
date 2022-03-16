using System;

namespace AccSaber.Models
{
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
}