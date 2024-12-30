using System;
using System.Collections.Generic;
using System.Threading;
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
		public event Action? OnUpdatingFromAccSaberAPI;
		public event Action<bool>? OnUpdatedFromAccSaberAPI;

		public Dictionary<string, AccSaberRankedMap> RankedMaps = new();
		private AccSaberUser _currentUserOverall = new();
		private AccSaberUser _currentUserTrue = new();
		private AccSaberUser _currentUserStandard = new();
		private AccSaberUser _currentUserTech = new();
		public  DateTime LastLocalUpdateTime { get; private set; } = DateTime.MinValue;
		
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
				rankedMaps[$"{map.SongHash}/{map.Difficulty}".ToLower()] = map;
			}

			return rankedMaps;
		}
		
		private async Task UpdateAccSaberInfo(DateTime? lastAPIUpdateTime = null)
		{
			OnUpdatingFromAccSaberAPI?.Invoke();

			lastAPIUpdateTime ??= await GetLastApiUpdateTime();
			LastLocalUpdateTime = lastAPIUpdateTime.Value;
			
			var platformUser = await GetPlatformUserInfo();
			if (platformUser is null)
			{
				_log.Error("platformUser is null");
				return;
			}

			var newOverall = await GetUserFromId(platformUser.platformUserId);
			
			// Check if the data fetched is the same as what we already have cached
			// Saves us from calling the API three more times for the True, Standard and Tech user categories.
			if (Math.Abs(newOverall.AP - _currentUserOverall.AP) < 0.01f)
			{
				OnUpdatedFromAccSaberAPI?.Invoke(false);
				return;
			}

			_currentUserOverall = newOverall;
			await Task.Delay(1000);
			_currentUserTrue = await GetUserFromId(platformUser.platformUserId, AccSaberMapCategories.True);
			await Task.Delay(1000);
			_currentUserStandard = await GetUserFromId(platformUser.platformUserId, AccSaberMapCategories.Standard);
			await Task.Delay(1000);
			_currentUserTech = await GetUserFromId(platformUser.platformUserId, AccSaberMapCategories.Tech);
			
			OnUpdatedFromAccSaberAPI?.Invoke(true);
		}
		private async Task<DateTime> GetLastApiUpdateTime()
		{
			var response = await _webUtils.GetAsync("https://api.accsaber.com/status/last-update");

			if (response is null)
			{
				return DateTime.MinValue;
			}
			
			// TODO: Replace this with ParseExact
			// The format just doesn't want to work GRAHHH
			/*_log.Error(await response.ReadAsStringAsync());
			
			const string format = "yyyy-MM-ddTHH:mm:ss.fffffffffZ";
			var lastApiUpdate = DateTime.ParseExact("2024-12-29T14:57:56.827733630", "yyyy-MM-ddTHH:mm:ss.fffffffff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);*/
			var lastApiUpdate = DateTime.Parse(await response.ReadAsStringAsync(), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);
			return lastApiUpdate;
		}
		
		public Task<AccSaberUser> GetCurrentUser(AccSaberMapCategories? category = null)
		{
			return Task.FromResult(category switch
			{
				AccSaberMapCategories.True => _currentUserTrue,
				AccSaberMapCategories.Standard => _currentUserStandard,
				AccSaberMapCategories.Tech => _currentUserTech,
				null => _currentUserOverall,
				_ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
			});
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
				response = await _webUtils.GetAsync<AccSaberUser>($"https://api.accsaber.com/players/{id}/{category.ToString().ToLower()}");
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
			return await _platformUserModel.GetUserInfo(CancellationToken.None);
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
		
		public AccSaberUser GetCurrentCategoryUser()
		{
			return _currentRankedMap?.Category switch
			{
				AccSaberMapCategories.True => _currentUserTrue,
				AccSaberMapCategories.Standard => _currentUserStandard,
				AccSaberMapCategories.Tech => _currentUserTech,
				_ => _currentUserOverall
			};
		}

		public async Task<bool> HasAccSaberUpdated()
		{
			// AccSaber updates every 30 minutes~, so no need to check if we know it updated say 5 minutes ago
			if (DateTime.UtcNow < LastLocalUpdateTime.AddMinutes(15))
			{
				return false;
			}
			
			var lastApiUpdate = await GetLastApiUpdateTime();
			
			if (lastApiUpdate <= LastLocalUpdateTime)
			{
				return false;
			}

			await UpdateAccSaberInfo(lastApiUpdate);
			return true;
		}
		
		public async void Initialize()
		{
			RankedMaps = await GetRankedMaps();
		}
	}
}