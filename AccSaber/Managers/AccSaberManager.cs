using AccSaber.Utils;
using SiraUtil.Logging;
using LeaderboardCore.Interfaces;

namespace AccSaber.Managers
{
	internal class AccSaberManager : INotifyLeaderboardSet
	{
		private readonly SiraLog _log;
		private readonly WebUtils _webUtils;
		private readonly AccSaberStore _accSaberStore;
        
		public AccSaberManager(SiraLog log, WebUtils webUtils, AccSaberStore accSaberStore)
		{
			_log = log;
			_webUtils = webUtils;
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