using System.Threading.Tasks;
using AccSaber.Configuration;
using AccSaber.Managers;
using AccSaber.Models;
using AccSaber.Utils;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using SiraUtil.Logging;
using Tweening;
using UnityEngine;
using Zenject;

namespace AccSaber.UI.ViewControllers
{
	[ViewDefinition("AccSaber.UI.Views.AccSaberPanelView.bsml")]
	[HotReload(RelativePathToLayout = @"..\UI\Views\AccSaberPanelView.bsml")]
	internal sealed class AccSaberPanelViewController : BSMLAutomaticViewController
	{
		[UIComponent("container")] private readonly Backgroundable _container = null!;

		private bool _parsed;
		private bool _firstShowing;
		private bool _loadingActive;
		private string _promptText = "";

		private SiraLog _log = null!;
		private PluginConfig _pluginConfig = null!;
		private AccSaberStore _accSaberStore = null!;
		private TimeTweeningManager _timeTweeningManager = null!;

		[Inject]
		public void Construct(SiraLog siraLog, PluginConfig pluginConfig, AccSaberStore accSaberStore, TimeTweeningManager timeTweeningManager)
		{
			_log = siraLog;
			_pluginConfig = pluginConfig;
			_accSaberStore = accSaberStore;
			_timeTweeningManager = timeTweeningManager;
		}
        
		private void AccSaberStoreOnOnAccSaberRankedMapUpdated(AccSaberRankedMap? mapInfo)
		{
			if (mapInfo is null)
			{
				return;
			}
            
			if (!_parsed || _pluginConfig.RainbowHeader)
			{
				return;
			}

			if (_firstShowing)
			{
				SetBannerColor(mapInfo.categoryDisplayName);
				_firstShowing = false;
			}
			else
			{
				TweenBannerColor(mapInfo.categoryDisplayName);	
			}
		}

		private void OnEnable()
		{
			_accSaberStore.OnAccSaberRankedMapUpdated += AccSaberStoreOnOnAccSaberRankedMapUpdated;
			
			if (_pluginConfig.RainbowHeader && _parsed)
			{
				_ = ToggleRainbowBannerTween(true);
			}
		}

		private void OnDisable()
		{
			_accSaberStore.OnAccSaberRankedMapUpdated -= AccSaberStoreOnOnAccSaberRankedMapUpdated;
			
			if (!_parsed)
			{
				return;
			}
			
			_firstShowing = true;
			_ = ToggleRainbowBannerTween(false);
		}

		private void SetBannerColor(string category)
		{
			if (_container.background is not ImageView background || _pluginConfig.RainbowHeader)
			{
				return;
			}
            
			var color = GetCategoryColor(category);
			background.color0 = color;
			background.color1 = color.ColorWithAlpha(0);
		}

		private void TweenBannerColor(string category)
		{
			if (_container.background is not ImageView background)
			{
				return;
			}

			_timeTweeningManager.KillAllTweens(this);
			
			var originalColor = background.color0;
			Color targetColor = GetCategoryColor(category);
			
			var firstTween = new ColorTween(originalColor, targetColor, val => background.color0 = val, 0.25f, EaseType.InSine);
			var secondTween = new ColorTween(originalColor, targetColor, val => background.color1 = val.ColorWithAlpha(0), 0.25f, EaseType.InSine, 0.15f);
            
			_timeTweeningManager.AddTween(firstTween, this);
			_timeTweeningManager.AddTween(secondTween, this);
		}

		private async Task ToggleRainbowBannerTween(bool enable)
		{
			if (_container.background is not ImageView background)
			{
				return;
			}

			if (enable)
			{
				var tween = new FloatTween(0f, 1, val => background.color0 = Color.HSVToRGB((val + 0.2f) % 1f, 1f, 1f), 6f, EaseType.Linear)
				{
					loop = true
				};
				var tween2 = new FloatTween(0f, 1, val => background.color1 = Color.HSVToRGB(val, 1f, 1f).ColorWithAlpha(0), 6f, EaseType.Linear)
				{
					loop = true
				};
				
				_timeTweeningManager.AddTween(tween, this);
				await Task.Delay(100);
				if (tween.isKilled)
				{
					return;
				}
				_timeTweeningManager.AddTween(tween2, this);
			}
			else
			{
				_timeTweeningManager.KillAllTweens(this);
				if (_accSaberStore.CurrentRankedMap?.categoryDisplayName != null)
				{
					SetBannerColor(_accSaberStore.CurrentRankedMap.categoryDisplayName);
				}
			}
		}

		private Color GetCategoryColor(string category)
		{
			return category switch
			{
				"True Acc" => new Color(0.015f, 0.906f, 0.176f, 1),
				"Standard Acc" => new Color(0.039f, 0.573f, 0.918f, 1),
				"Tech Acc" => new Color(0.902f, 0.027f, 0.027f, 1),
				_ => Color.gray
			};
		}

		[UIAction("#post-parse")]
		public void PostParse()
		{
			if (_container.background is ImageView background)
			{
				background.material = Utilities.ImageResources.NoGlowMat;
                background.color = Color.gray;

				Accessors.GradientAccessor(ref background) = true;
				Accessors.SkewAccessor(ref background) = 0.18f;
			}

			_parsed = true;
            
			if (_accSaberStore.CurrentRankedMap is not null)
			{
				if (_pluginConfig.RainbowHeader)
				{
					_ = ToggleRainbowBannerTween(true);
					return;
				}
				
				SetBannerColor(_accSaberStore.CurrentRankedMap.categoryDisplayName);
				_firstShowing = false;
			}
		}
		
		[UIValue("loading-active")]
		public bool LoadingActive
		{
			get => false;
			set
			{
				// _loadingActive = value;
				// NotifyPropertyChanged(nameof(LoadingActive));
			}
		}

		[UIValue("prompt-text")]
		public string PromptText
		{
			get => "_promptText";
			set
			{
				// _promptText = value;
				// NotifyPropertyChanged(nameof(PromptText));
			}
		}

		[UIValue("pool-ranking-text")]
		private string PoolRankingText =>
			$"<color=#EDFF55>Category Ranking:</color> #_userModel.rank <size=75%>(<color=#00FFAE>_userModel.ap:F2ap</color>)";
        
		[UIValue("average-acc-text")]
		private string AverageAccText => $"<color=#EDFF55>Map Complexity:</color> _APISong.complexity";
	}
}