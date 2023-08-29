using System;
using System.Collections.Generic;
using AccSaber.Models;
using SiraUtil.Logging;
using LeaderboardCore.Interfaces;

namespace AccSaber.Managers
{
	internal class AccSaberManager : INotifyLeaderboardSet
	{
		private readonly SiraLog _log;
		private readonly Downloader _downloader;
		private readonly AccSaberStore _accSaberStore;
        
		public AccSaberManager(SiraLog log, Downloader downloader, AccSaberStore accSaberStore)
		{
			_log = log;
			_downloader = downloader;
			_accSaberStore = accSaberStore;
		}
        
		public void OnLeaderboardSet(IDifficultyBeatmap? difficultyBeatmap)
		{
			if (difficultyBeatmap is not {level: CustomPreviewBeatmapLevel level})
			{
				return;
			}

			var hash = $"{SongCore.Utilities.Hashing.GetCustomLevelHash(level)}/{difficultyBeatmap.difficulty}".ToLower();
			var mapInfo = _accSaberStore.RankedMaps.TryGetValue(hash, out var ret) ? ret : null;

			_accSaberStore.CurrentRankedMap = mapInfo;
		}
	}
}