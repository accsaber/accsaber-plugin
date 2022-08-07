using System;
using System.ComponentModel;
using System.Linq;
using AccSaber.Data;
using AccSaber.Models;
using AccSaber.UI.MenuButton;
using AccSaber.Utils;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;
using Color = UnityEngine.Color;
using AccSaberCategoryColor = AccSaber.Utils.AccSaberUtils;

namespace AccSaber.UI.Panel
{
    [ViewDefinition("AccSaber.UI.Panel.AccSaberPanelView.bsml")]
    [HotReload(RelativePathToLayout = @"..\UI\Panel\AccSaberPanelView.bsml")]
    public class AccSaberPanelViewController : BSMLAutomaticViewController, IInitializable, IDisposable, INotifyPropertyChanged
    {
        [Inject] private AccSaberMainFlowCoordinator _accSaberMainFlowCoordinator;

        [Inject] private SiraLog _siraLog;

        [Inject] private AccSaberUserModel _userModel;

        [Inject] private AccSaberData _accSaberData;

        [Inject] private AccSaberAPISong _APISong;
        
        private AccSaberSong _song;

        [Inject] private LevelCollectionNavigationController _navigation;

        private Sprite _logoSprite;

        [UIComponent("container")] private readonly Backgroundable backgroundable;

        [UIComponent("logo")] private ImageView accSaberlogo;

        [UIComponent("download-image")] private ClickableImage downloadImage;

        [UIComponent("separator")] private ImageView separator;

        private string _promptText = "";
        private bool _loadingActive;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Initialize()
        {
        }

        [UIAction("#post-parse")]
        public void PostParse()
        {
            _siraLog.Info("Post-parsing started..");
            
            if (backgroundable.background is ImageView background)
            {
                background.material = Utilities.ImageResources.NoGlowMat;
                RefreshAndSkewBannerColor();

                background.color = Color.gray;

                Accessors.GradientAccessor(ref background) = true;
                Accessors.SkewAccessor(ref background) = 0.18f;
            }

            if (_logoSprite != null)
            {
                _logoSprite = accSaberlogo.sprite;
            }

            if (accSaberlogo != null)
            {
                Accessors.SkewAccessor(ref accSaberlogo) = 0.18f;
                accSaberlogo.SetVerticesDirty();
            }

            if (separator != null)
            {
                Accessors.SkewAccessor(ref separator) = 0.18f;
                separator.SetVerticesDirty();
            }
        }

        public void RefreshAndSkewBannerColor()
        {
            try
            {
                if (backgroundable.background is ImageView background)
                {
                    switch (_accSaberData.RankedMaps.Single(x => 
                        String.Equals(x.songHash, _navigation.selectedDifficultyBeatmap.level.levelID.GetRankedSongHash(), StringComparison.CurrentCultureIgnoreCase) 
                        && String.Equals(x.difficulty, _navigation.selectedDifficultyBeatmap.difficulty.ToString(), StringComparison.CurrentCultureIgnoreCase)).categoryDisplayName)
                    {
                        case "True Acc":
                            background.color0 = new Color(0.015f, 0.906f, 0.176f, 1);
                            background.color1 = new Color(0.015f, 0.906f, 0.176f, 0);
                            break;
                        case "Standard Acc":
                            background.color0 = new Color(0.039f, 0.573f, 0.918f, 1);
                            background.color1 = new Color(0.039f, 0.573f, 0.918f, 0);
                            break;
                        case "Tech Acc":
                            background.color0 = new Color(0.902f, 0.027f, 0.027f, 1);
                            background.color1 = new Color(0.902f, 0.027f, 0.027f, 0);
                            break;
                        default:
                            _siraLog.Debug("No hash matching a known AccSaber hash, skipping.");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                _siraLog.Debug(e);
            }
            
        }

        [UIValue("loading-active")]
        public bool LoadingActive
        {
            get => _loadingActive;
            set
            {
                _loadingActive = value;
                NotifyPropertyChanged(nameof(LoadingActive));
            }
        }

        [UIValue("prompt-text")]
        public string PromptText
        {
            get => _promptText;
            set
            {
                _promptText = value;
                NotifyPropertyChanged(nameof(PromptText));
            }
        }

        [UIValue("pool-ranking-text")]
        private string PoolRankingText =>
            $"<color=#EDFF55>Category Ranking:</color> #{_userModel.rank} <size=75%>(<color=#00FFAE>{_userModel.ap:F2}ap</color>)";

        [UIValue("average-acc-text")]
        private string AverageAccText => $"<color=#EDFF55>Map Complexity:</color> {_APISong.complexity}";

        [UIValue("download-hover")]
        private string DownloadHint => "Download all maps, including the ones \n that have been updated!";

        [UIAction("download-click")]
        private void DownloadClick() => _accSaberMainFlowCoordinator.Show();

        public void Dispose()
        {
        }
    }
}