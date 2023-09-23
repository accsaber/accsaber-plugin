using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccSaber.Models;
using AccSaber.Utils;
using SiraUtil.Logging;
using Zenject;

namespace AccSaber.Managers
{
	internal sealed class AccSaberStore : IInitializable
	{
		private readonly SiraLog _log;
		private readonly WebUtils _webUtils;
		private readonly IPlatformUserModel _platformUserModel;
		
		public event Action<AccSaberRankedMap?>? OnAccSaberRankedMapUpdated; 

		public Dictionary<string, AccSaberRankedMap> RankedMaps = new();
		private AccSaberUser _currentUser = null!;
		private AccSaberUser _currentUserTrue = null!;
		private AccSaberUser _currentUserStandard = null!;
		private AccSaberUser _currentUserTech = null!;
		
		private AccSaberRankedMap? _currentRankedMap;

		public AccSaberStore(SiraLog log, WebUtils webUtils, IPlatformUserModel platformUserModel)
		{
			_log = log;
			_webUtils = webUtils;
			_platformUserModel = platformUserModel;
		}
		
		public AccSaberRankedMap? CurrentRankedMap
		{
			get => _currentRankedMap;
			set
			{
				_currentRankedMap = value;
				OnAccSaberRankedMapUpdated?.Invoke(_currentRankedMap);
			}
		}

		private async Task<Dictionary<string, AccSaberRankedMap>> GetRankedMaps()
		{
			var response = await _webUtils.GetAsync<List<AccSaberRankedMap>>("https://api.accsaber.com/ranked-maps/");
			
			if (response == null)
			{
				_log.Error("Failed to get ranked maps from AccSaber API");
				return new Dictionary<string, AccSaberRankedMap>();
			}

			var rankedMaps = new Dictionary<string, AccSaberRankedMap>();
			foreach (var map in response)
			{
				rankedMaps[$"{map.songHash}/{map.difficulty}".ToLower()] = map;
			}

			return rankedMaps;
		}
		
		private async Task<AccSaberUser> GetCurrentUser(string? category = null)
		{
			var userInfo = await GetUserInfo();
			if (userInfo is null)
			{
				_log.Error("userInfo is null");
				return new AccSaberUser();
			}

			AccSaberUser? response;
			if (category is null)
			{
				response = await _webUtils.GetAsync<AccSaberUser>($"https://api.accsaber.com/players/{userInfo.platformUserId}", default);
			}
			else
			{
				response = await _webUtils.GetAsync<AccSaberUser>($"https://api.accsaber.com/players/{userInfo.platformUserId}/{category}");
			}
			
			if (response == null)
			{
				_log.Error("Failed to get current user from AccSaber API");
				return new AccSaberUser();
			}
            
			return response;
		}

		public async Task<UserInfo?> GetUserInfo()
		{
			// GetUserInfo caches the result
			return await _platformUserModel.GetUserInfo();
		}

		public AccSaberUser GetCurrentCategoryUser()
		{
			return _currentRankedMap?.categoryDisplayName switch
			{
				"True Acc" => _currentUserTrue,
				"Standard Acc" => _currentUserStandard,
				"Tech Acc" => _currentUserTech,
				_ => _currentUser
			};
		}

		public async void Initialize()
		{
			RankedMaps = await GetRankedMaps();
			_currentUser = await GetCurrentUser();
			_currentUserTrue = await GetCurrentUser("true");
			_currentUserStandard = await GetCurrentUser("standard");
			_currentUserTech = await GetCurrentUser("tech");
		}
	}
}