using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using IPA.Utilities;
using SiraUtil.Tools;
using AccSaber.Models;

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

        [UIValue("acc-list")]
        private List<object> AccOptions = Enum.GetValues(typeof(AccTypesList.AccTypes)).Cast<object>().ToList();
        
        // [UIValue("list-choice")]
        // private string listChoice = "True";
    }
}