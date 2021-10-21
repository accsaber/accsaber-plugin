using System.Collections.Generic;

namespace AccSaber.Utils
{
    public static class BeatSaverUtils
    {
        public class BeatSaverAPISong
        {
            public List<BeatSaverAPIVersion> versions;
        }

        public class BeatSaverAPIVersion
        {
            public string hash;
            public string downloadURL;
        }
    }
}
