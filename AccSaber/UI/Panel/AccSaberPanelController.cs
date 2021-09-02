using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using IPA.Utilities;
using SiraUtil.Tools;

namespace AccSaber.UI.Leaderboard.Panel
{
    [ViewDefinition("AccSaber.UI.Panel.HeaderPanel.bsml")]
    [HotReload(RelativePathToLayout = @".\HeaderPanel.bsml")]
    public class AccSaberPanelController : BSMLAutomaticViewController
    {
        private SiraLog _log;
        
        private AccSaberPanelController(SiraLog log)
        {
            _log = log;
        }

        private enum AccTypes
        {
            True,
            Standard,
            Tech
        }

        [UIValue("acc-list")]
        private List<object> AccOptions = Enum.GetValues(typeof(AccTypes)).Cast<object>().ToList();
    }
}