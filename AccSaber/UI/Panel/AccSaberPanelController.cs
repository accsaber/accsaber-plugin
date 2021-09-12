using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AccSaber.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using LeaderboardCore.Models.UI.ViewControllers;
using SiraUtil;
using SiraUtil.Tools;
using Zenject;

namespace AccSaber.UI.Panel
{
    public class AccSaberPanelController : BasicPanelViewController
    {
        protected override string LogoSource => "AccSaber.Resources.Logos.AccSaber.png";
        protected string ExtraResourceName => "AccSaber.UI.Panel.HeaderPanel.bsml";
        protected override string customBSML => 
            Utilities.GetResourceContent(Assembly.GetAssembly(typeof(AccSaberPanelController)),
                ExtraResourceName);
        protected override object customHost => this;
    }
}