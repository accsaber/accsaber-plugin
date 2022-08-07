using AccSaber.Downloaders;
using AccSaber.Interfaces;
using AccSaber.Utils;
using HMUI;
using UnityEngine;

namespace AccSaber.Sources
{
    public class AroundMeLeaderboardSource : ILeaderboardSource
    {
        public string HoverHint => "Around Me";
        public Sprite _icon;
        
        public Sprite Icon
        {
            get
            {
                if (_icon == null)
                {
                    _icon = BeatSaberMarkupLanguage.Utilities.FindSpriteInAssembly("AccSaber.Resources.PlayerIcon.png");
                }
                return _icon;
            }
        }

        public bool Scrollable => false;
    }
}