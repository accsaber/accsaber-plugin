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
        
        private Sprite _logoSprite;
        private Sprite _flushedSprite;

        [UIComponent("container")]
        private readonly Backgroundable backgroundable;

        [UIComponent("logo")] 
        private ImageView accSaberlogo;
        
        [UIComponent("separator")]
        private ImageView separator;
       
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
                    _siraLog.Info("1");
                    background.color0 = new Color(0.015f, 0.906f, 0.176f, 1);
                    _siraLog.Info("2");
                    background.color1 = new Color(0.015f, 0.906f, 0.176f, 0);
                    _siraLog.Info("3");
                    background.color = Color.white;
                    _siraLog.Info("4");
                    Accessors.GradientAccessor(ref background) = true;
                    _siraLog.Info("5");
                    Accessors.SkewAccessor(ref background) = 0.18f;
                    _siraLog.Info("finished parsing the backgroundable");
                }
            }
        
            _logoSprite = accSaberlogo.sprite;
            _flushedSprite =
                BeatSaberMarkupLanguage.Utilities.FindSpriteInAssembly("AccSaber.Resources.Logos.AccSaber.png");
            _siraLog.Info("knobheadthefirst");
        
            if (accSaberlogo != null)
            {
                _siraLog.Info("knobhead");
                Accessors.SkewAccessor(ref accSaberlogo) = 0.18f;
                accSaberlogo.SetVerticesDirty();
            }
        
            if (separator != null)
            {
                _siraLog.Info("knobhead2");
                Accessors.SkewAccessor(ref separator) = 0.18f;
                separator.SetVerticesDirty();    
            }
        }

        public void Dispose()
        {
        }
    }
}