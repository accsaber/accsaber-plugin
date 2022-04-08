using System;
using AccSaber.Models;
using AccSaber.Utils;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using JetBrains.Annotations;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;
using Color = UnityEngine.Color;

namespace AccSaber.UI.Panel
{
    [ViewDefinition("AccSaber.UI.Panel.AccSaberPanelView.bsml")]
    [HotReload(RelativePathToLayout = @"..\UI\Panel\AccSaberPanelView.bsml")]
    public class AccSaberPanelController : BSMLAutomaticViewController, IInitializable, IDisposable
    {
        [Inject]
        private SiraLog _siraLog;

        [Inject] 
        private AccSaberUserModel _userModel;

        [Inject] 
        private AccSaberCategory _category;
        
        private Sprite _logoSprite;
        private Sprite _flushedSprite;

        [UIComponent("container")]
        private readonly Backgroundable backgroundable;

        [UIComponent("logo")] 
        private ImageView accSaberlogo;
        
        [UIComponent("separator")]
        private ImageView separator;
        
        private string promptText = "";
        private bool loadingActive;
       
        public void Initialize()
        {
            
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            _siraLog.Info("Post-parsing started..");
            if (backgroundable.background is ImageView background)
            {
                if (background != null)
                {
                    background.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
                    background.color0 = new Color(0.902f, 0.027f, 0.027f, 1);
                    background.color1 = new Color(1f, 1, 1f, 0.01f);
                    background.color = Color.gray;
                    Accessors.GradientAccessor(ref background) = true;
                    Accessors.SkewAccessor(ref background) = 0.18f;
                }
            }

            if (_logoSprite != null)
            {
                _logoSprite = accSaberlogo.sprite;
                _flushedSprite =
                    BeatSaberMarkupLanguage.Utilities.FindSpriteInAssembly("AccSaber.Resources.Logos.AccSaber.png");
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
            get => loadingActive;
            set
            {
                loadingActive = value;
                NotifyPropertyChanged(nameof(LoadingActive));
            }
        }
        
        [UIValue("prompt-text")]
        public string PromptText
        {
            get => promptText;
            set
            {
                promptText = value;
                NotifyPropertyChanged(nameof(PromptText));
            }
        }
        
        [UIValue("pool-ranking-text")]
        private string PoolRankingText => $"<b>Category Ranking:</b> #{_userModel.rank} <size=75%>(<color=#aa6eff>{_userModel.ap:F2}ap</color>)";

        public void Dispose()
        {
        }
    }
}