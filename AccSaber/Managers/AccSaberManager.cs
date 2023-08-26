using System;
using System.Collections.Generic;
using AccSaber.Models;
using SiraUtil.Logging;
using AccSaber.Interfaces;
using LeaderboardCore.Interfaces;
using Zenject;

namespace AccSaber.Managers
{
	internal class AccSaberManager : IInitializable, INotifyLeaderboardSet
	{
		private readonly SiraLog _log;
		private readonly Downloader _downloader;
		private readonly List<INotifyDifficultyBeatmapUpdated> _notifyDifficultyBeatmapUpdateds;
        
		public AccSaberManager(SiraLog log, Downloader downloader, List<INotifyDifficultyBeatmapUpdated> notifyDifficultyBeatmapUpdateds)
		{
			_log = log;
			_downloader = downloader;
			_notifyDifficultyBeatmapUpdateds = notifyDifficultyBeatmapUpdateds;
		}
        
		private Dictionary<string, AccSaberRankedMap> _rankedMaps = new();

		public async void Initialize()
		{
			var response = await _downloader.Get<List<AccSaberRankedMap>>("https://api.accsaber.com/ranked-maps/");
			
			if (response == null)
			{
				_log.Error("Failed to get ranked maps from AccSaber API");
				return;
			}

			foreach (var map in response)
			{
				_rankedMaps[$"{map.songHash}/{map.difficulty}".ToLower()] = map;
			}
		}

		public void OnLeaderboardSet(IDifficultyBeatmap? difficultyBeatmap)
		{
			if (difficultyBeatmap is not {level: CustomPreviewBeatmapLevel level})
			{
				return;
			}

			var hash = $"{SongCore.Utilities.Hashing.GetCustomLevelHash(level)}/{difficultyBeatmap.difficulty}".ToLower();
			var mapInfo = _rankedMaps.TryGetValue(hash, out var ret) ? ret : null;
			
			foreach (var notifyDifficultyBeatmapUpdated in _notifyDifficultyBeatmapUpdateds)
			{
				notifyDifficultyBeatmapUpdated.DifficultyBeatmapUpdated(difficultyBeatmap, mapInfo);
			}
		}
	}
}