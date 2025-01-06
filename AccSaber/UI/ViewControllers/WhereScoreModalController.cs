using System.Linq;
using System.Reflection;
using AccSaber.Utils;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using UnityEngine;

namespace AccSaber.UI.ViewControllers
{
    internal sealed class WhereScoreModalController
    {
        private bool _parsed;
        
        [UIComponent("modal")]
        private ModalView _modalView = null!;
        
        [UIParams]
        private readonly BSMLParserParams _parserParams = null!;
        
        private void Parse(Transform parentTransform)
        {
            if (!_parsed)
            {
                BSMLParser.Instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "AccSaber.UI.Views.WhereScoreModalView.bsml"), parentTransform.gameObject, this);
                _modalView.name = "AccSaberLeaderboardWhereScoreModal";
                
                _parsed = true;
            }
			
            _modalView.transform.SetParent(parentTransform.transform);
            Accessors.ViewValidAccessor(ref _modalView) = false;
        }
		
        public void ShowModal(Transform parentTransform)
        {
            Parse(parentTransform);
            
            _parserParams.EmitEvent("close-modal");
            _parserParams.EmitEvent("open-modal");
        }

        public void HideModal()
        {
            if (!_parsed)
            {
                return;
            }
			
            _parserParams.EmitEvent("close-modal");
        }
    }
}