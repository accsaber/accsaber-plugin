using System.Reflection;
using BeatSaberMarkupLanguage;
using LeaderboardCore.Models.UI.ViewControllers;
using SiraUtil;

namespace AccSaber.UI.Panel
{
    public class AccSaberPanelController : BasicPanelViewController
    {
        protected override string LogoSource => "AccSaber.Resources.Logos.AccSaber.png";
        private string ExtraResourceName => "AccSaber.UI.Panel.HeaderPanel.bsml";

        protected override string customBSML =>
            Utilities.GetResourceContent(Assembly.GetAssembly(typeof(AccSaberPanelController)),
                ExtraResourceName);

        protected override object customHost => this;
    }
}