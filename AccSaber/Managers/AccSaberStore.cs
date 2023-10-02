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
		public event Action? OnFetchingCurrentUser;
		public event Action<bool> OnFetchedCurrentUser;

		public Dictionary<string, AccSaberRankedMap> RankedMaps = new();
		private AccSaberUser _currentUserOverall = new();
		private AccSaberUser _currentUserTrue = new();
		private AccSaberUser _currentUserStandard = new();
		private AccSaberUser _currentUserTech = new();
		private DateTime _lastRefresh;
		
		private AccSaberRankedMap? _currentRankedMap;

		public AccSaberStore(SiraLog log, WebUtils webUtils, IPlatformUserModel platformUserModel)
		{
			_log = log;
			_webUtils = webUtils;
			_platformUserModel = platformUserModel;
		}

		public enum AccSaberMapCategories
		{
			True,
			Standard,
			Tech
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

		public bool IsStoredUserValid()
		{
			return _lastRefresh.AddMinutes(20) > DateTime.Now;
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
		
		private async Task UpdateUserInfo()
		{
			OnFetchingCurrentUser?.Invoke();
			
			_lastRefresh = DateTime.Now;
			var platformUser = await GetPlatformUserInfo();
			if (platformUser is null)
			{
				_log.Error("platformUser is null");
				return;
			}

			var newOverall = await GetUserFromId(platformUser.platformUserId);
			
			// Check if the data fetched is the same as what we already have cached
			// Saves us from calling the API three more times for the True, Standard and Tech user categories.
			if (Math.Abs(newOverall.ap - _currentUserOverall.ap) < 0.01f)
			{
				OnFetchedCurrentUser?.Invoke(false);
				return;
			}

			_currentUserOverall = newOverall;
			await Task.Delay(1000);
			_currentUserTrue = await GetUserFromId(platformUser.platformUserId, AccSaberMapCategories.True);
			await Task.Delay(1000);
			_currentUserStandard = await GetUserFromId(platformUser.platformUserId, AccSaberMapCategories.Standard);
			await Task.Delay(1000);
			_currentUserTech = await GetUserFromId(platformUser.platformUserId, AccSaberMapCategories.Tech);
			
			OnFetchedCurrentUser?.Invoke(true);
		}
		
		public async Task<AccSaberUser> GetCurrentUser(AccSaberMapCategories? category = null)
		{
			if (!IsStoredUserValid())
			{
				await UpdateUserInfo();
			}

			return category switch
			{
				AccSaberMapCategories.True => _currentUserTrue,
				AccSaberMapCategories.Standard => _currentUserStandard,
				AccSaberMapCategories.Tech => _currentUserTech,
				null => _currentUserOverall,
				_ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
			};
		}

		public async Task<AccSaberUser> GetUserFromId(string id, AccSaberMapCategories? category = null)
		{
			AccSaberUser? response;
			if (category is null)
			{
				response = await _webUtils.GetAsync<AccSaberUser>($"https://api.accsaber.com/players/{id}");
			}
			else
			{
				response = await _webUtils.GetAsync<AccSaberUser>($"https://api.accsaber.com/players/{id}/{category}");
			}

			if (response != null)
			{
				return response;
			}

			_log.Error($"Failed to get user {id} from AccSaber API");
			return new AccSaberUser();

		}

		public async Task<UserInfo?> GetPlatformUserInfo()
		{
			// GetUserInfo caches the result, no need to do it ourselves
			return await _platformUserModel.GetUserInfo();
		}

		public async Task<AccSaberUser> GetCurrentCategoryUserAsync()
		{
			return _currentRankedMap?.Category switch
			{
				AccSaberMapCategories.True => await GetCurrentUser(AccSaberMapCategories.True),
				AccSaberMapCategories.Standard => await GetCurrentUser(AccSaberMapCategories.Standard),
				AccSaberMapCategories.Tech => await GetCurrentUser(AccSaberMapCategories.Tech),
				_ => await GetCurrentUser()
			};
		}
		
		/// <summary>
		/// Gets category user from cached data, could return outdated user.
		/// Check <see cref="IsStoredUserValid"/>, otherwise use <see cref="GetCurrentCategoryUserAsync"/>.
		/// </summary>
		public AccSaberUser GetCurrentCategoryUser()
		{
			if (!IsStoredUserValid())
			{
				_ = UpdateUserInfo();
			}
			
			return _currentRankedMap?.Category switch
			{
				AccSaberMapCategories.True => _currentUserTrue,
				AccSaberMapCategories.Standard => _currentUserStandard,
				AccSaberMapCategories.Tech => _currentUserTech,
				_ => _currentUserOverall
			};
		}
		
		public async void Initialize()
		{
			_lastRefresh = DateTime.Now;
			// Not too sure if we need to refresh the ranked map list
			// Chances of the ranked map list becoming outdated is pretty low
			RankedMaps = await GetRankedMaps();
			await Task.Delay(1000);
			await UpdateUserInfo();
		}
	}
}