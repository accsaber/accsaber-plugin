using System;
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

namespace AccSaber.UI.Panel
{
    [ViewDefinition("AccSaber.UI.Panel.AccSaberPanelView.bsml")]
    [HotReload(RelativePathToLayout = @"..\UI\Panel\AccSaberPanelView.bsml")]
    public class AccSaberPanelViewController : BSMLAutomaticViewController, IInitializable, IDisposable
    {
        [Inject] 
        private AccSaberMainFlowCoordinator _accSaberMainFlowCoordinator;
        
        [Inject]
        private SiraLog _siraLog;

        [Inject] 
        private AccSaberUserModel _userModel;
        
        [Inject]
        private AccSaberData _accSaberData;
        
        [Inject]
        private AccSaberAPISong _APISong;

        [Inject]
        private AccSaberSongDiff _songDiff;

        [Inject] 
        private AccSaberSong _selectedSong;

        [Inject] 
        private AccSaberCategory _category;

        [Inject] 
        private LevelCollectionNavigationController _navigation;
        
        private Sprite _logoSprite;

        [UIComponent("container")]
        private readonly Backgroundable backgroundable;

        [UIComponent("logo")] 
        private ImageView accSaberlogo;

        [UIComponent("download-image")] 
        private ClickableImage downloadImage;
        
        [UIComponent("separator")]
        private ImageView separator;
        
        private string _promptText = "";
        private bool _loadingActive;

        public void Initialize()
        {
            
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            var rankedSong = _navigation.selectedDifficultyBeatmap;
            
            _siraLog.Info("Post-parsing started..");
            if (backgroundable.background is ImageView background)
            {
                if (background != null)
                {
                    background.material = Utilities.ImageResources.NoGlowMat;
                    Accessors.GradientAccessor(ref background) = true;
                    Accessors.SkewAccessor(ref background) = 0.18f;

                    BeatmapDifficulty difficulty;
                    foreach (var category in _songDiff.difficulty)
                    {
                        background.color0 = 
                            background.color1 = new Color(1f, 1f, 1f, 0f);
                        background.color = Color.gray;
                    }
                }
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
        private string PoolRankingText => $"<color=#EDFF55>Category Ranking:</color> #{_userModel.rank} <size=75%>(<color=#00FFAE>{_userModel.ap:F2}ap</color>)";
        
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