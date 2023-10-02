// TODO: Add ACC Campaign badges when the API properly exposes the information

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AccSaber.Managers;
using AccSaber.Models;
using AccSaber.Utils;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using IPA.Utilities;
using UnityEngine;

namespace AccSaber.UI.ViewControllers
{
	internal sealed class LeaderboardUserModalController : INotifyPropertyChanged, IDisposable
	{
		private string? _userId;
		private AccSaberUser? _userOverall;
		private AccSaberUser? _userTrue;
		private AccSaberUser? _userStandard;
		private AccSaberUser? _userTech;
		private bool _parsed;
		private bool _firstLoad;
		private bool _isLoading;
		private string _categoryValue = "Overall";
		private string _username = "";
		private string _rank = null!;
		private string _ap = null!;
		private string _plays = null!;
		private string _headset = null!;
		
		public event PropertyChangedEventHandler? PropertyChanged;
		
		[UIComponent("modal")]
		private ModalView _modalView = null!;

		[UIComponent("category-dropdown")]
		private readonly Transform _categoryDropdown = null!;

		[UIComponent("profile-image")]
		private readonly ImageView _profileImage = null!;

		[UIParams]
		private readonly BSMLParserParams _parserParams = null!;
		
        private readonly AccSaberStore _accSaberStore;
		
		public LeaderboardUserModalController(AccSaberStore accSaberStore)
		{
			_accSaberStore = accSaberStore;
		}
		
		#region UI Values
		
		[UIValue("is-loading")]
		private bool IsLoading
		{
			get => _isLoading;
			set
			{
				_isLoading = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotLoading)));
			}
		}

		[UIValue("is-not-loading")]
		private bool IsNotLoading => !_isLoading;

		[UIValue("category-value")]
		private string CategoryValue
		{
			get => _categoryValue;
			set
			{
				_categoryValue = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CategoryValue)));
				_ = UpdateUserInfo();
			}
		}

		[UIValue("category-choices")]
		private List<object> _categoryChoices = new() { "Overall", "True", "Standard", "Tech" };

		[UIValue("username")]
		private string Username
		{
			get => _username.Length > 18 ? $"{_username.Substring(0, 15)}..." : _username;
			set
			{
				_username = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Username)));
			}
		}
		
		[UIValue("rank")]
		private string Rank
		{
			get => _rank;
			set
			{
				_rank = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rank)));
			}
		}

		[UIValue("ap")]
		private string Ap
		{
			get => _ap;
			set
			{
				_ap = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Ap)));
			}
		}

		[UIValue("plays")]
		private string Plays
		{
			get => _plays;
			set
			{
				_plays = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Plays)));
			}
		}
		
		[UIValue("headset")]
		private string Headset
		{
			get => _headset;
			set
			{
				_headset = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Headset)));
			}
		}
		
		#endregion

		[UIAction("format-category")]
		private string FormatCategory(string value)
		{
			if (value == "Overall")
			{
				return value;
			}

			return value + " Acc";
		}

		private void Parse(Transform parentTransform)
		{
			if (!_parsed)
			{
				BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "AccSaber.UI.Views.LeaderboardUserModal.bsml"), parentTransform.gameObject, this);
				_modalView.name = "AccSaberLeaderboardUserModal";
				_modalView.blockerClickedEvent += OnModalClosed;
				
				var canvasGroup = _modalView.gameObject.AddComponent<CanvasGroup>();
				var dropdownModalView = _categoryDropdown.Find("DropdownTableView").GetComponent<ModalView>();
				dropdownModalView.SetupView(_modalView.transform);
				dropdownModalView.SetField("_parentCanvasGroup", canvasGroup);
				
				_profileImage.material = Resources.FindObjectsOfTypeAll<Material>().Last(x => x.name == "UINoGlowRoundEdge");
				
				_parsed = true;
			}
			
			_modalView.transform.SetParent(parentTransform.transform);
			Accessors.ViewValidAccessor(ref _modalView) = false;
		}
		
		public void ShowModal(Transform parentTransform, string userId)
		{
			Parse(parentTransform);

			_userId = userId;
			_firstLoad = true;
			
			CategoryValue = "Overall";
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
			OnModalClosed();
		}

		// TODO: Feedback says the details changing immediately from cached data can be jarring
		// Maybe add an animation to text whenever it's changed?, should be able to use AnimateCanvasAccessor from Accessors.cs
		// Would be better than showing the loading icon for a second.
		private async Task UpdateUserInfo()
		{
			if (_userId is null)
			{
				return;
			}
			
			// Rewrite this if statement mayhaps? :clueless:
			var platformUserInfo = await _accSaberStore.GetPlatformUserInfo();
			if (_userId == platformUserInfo?.platformUserId)
			{
				if (!_accSaberStore.IsStoredUserValid())
				{
					IsLoading = true;
				}
				
				switch (CategoryValue)
				{
					case "Overall":
					{
						var userInfo = await _accSaberStore.GetCurrentUser();
						_userOverall = userInfo;
						SetUserInfo(_userOverall);
						break;
					}
					case "True":
					{
						var userInfo = await _accSaberStore.GetCurrentUser(AccSaberStore.AccSaberMapCategories.True);
						_userTrue = userInfo;
						SetUserInfo(_userTrue);
						break;
					}
					case "Standard":
					{
						var userInfo = await _accSaberStore.GetCurrentUser(AccSaberStore.AccSaberMapCategories.Standard);
						_userStandard = userInfo;
						SetUserInfo(_userStandard);
						break;
					}
					case "Tech":
					{
						var userInfo = await _accSaberStore.GetCurrentUser(AccSaberStore.AccSaberMapCategories.Tech);
						_userTech = userInfo;
						SetUserInfo(_userTech);
						break;
					}
				}

				return;
			}
			
			switch (CategoryValue)
			{
				case "Overall":
				{
					if (_userOverall is null)
					{
						IsLoading = true;
						var userInfo = await _accSaberStore.GetUserFromId(_userId);
						_userOverall = userInfo;
					}

					SetUserInfo(_userOverall);
					
					break;
				}
				case "True":
				{
					if (_userTrue is null)
					{
						IsLoading = true;
						var userInfo = await _accSaberStore.GetUserFromId(_userId, AccSaberStore.AccSaberMapCategories.True);
						_userTrue = userInfo;
					}

					SetUserInfo(_userTrue);
					break;
				}
				case "Standard":
				{
					if (_userStandard is null)
					{
						IsLoading = true;
						var userInfo = await _accSaberStore.GetUserFromId(_userId, AccSaberStore.AccSaberMapCategories.Standard);
						_userStandard = userInfo;
					}

					SetUserInfo(_userStandard);
					break;
				}
				case "Tech":
				{
					if (_userTech is null)
					{
						IsLoading = true;
						var userInfo = await _accSaberStore.GetUserFromId(_userId, AccSaberStore.AccSaberMapCategories.Tech);
						_userTech = userInfo;
					}

					SetUserInfo(_userTech);
					break;
				}
			}
		}

		private void SetUserInfo(AccSaberUser userInfo)
		{
			Username = userInfo.playerName;
			Rank = $"#{userInfo.rank}";
			Ap = $"{userInfo.ap:N2} AP";
			Plays = $"{userInfo.rankedPlays} ranked plays";
			Headset = userInfo.hmd ?? "";

			if (_firstLoad)
			{
				_profileImage.SetImage(userInfo.avatarUrl, false, new BeatSaberUI.ScaleOptions(), () => IsLoading = false);	
			}
			else
			{
				_firstLoad = false;
				IsLoading = false;
			}
		}

		private void OnModalClosed()
		{
			_userId = null;
			_userOverall = null;
			_userTrue = null;
			_userStandard = null;
			_userTech = null;
		}

		public void Dispose()
		{
			_modalView.blockerClickedEvent -= OnModalClosed;
		}
	}
}