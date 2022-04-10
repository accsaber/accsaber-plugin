using System.Threading.Tasks;
using AccSaber.Downloaders;
using AccSaber.Interfaces;
using AccSaber.Models;
using UnityEngine;

namespace AccSaber.Sources
{
    public class GlobalLeaderboardSource : ILeaderboardSource
    {
        public string HoverHint => "Global";
        private Sprite _icon;
        public Sprite Icon
        {
            get
            {
                if (_icon == null)
                {
                    _icon = BeatSaberMarkupLanguage.Utilities.FindSpriteInAssembly("AccSaber.Resources.GlobalIcon.png");
                }
                return _icon;
            }
        }
        public AccSaberDownloader AccSaberDownloader { get; }
        public bool Scrollable => true;
    }
}