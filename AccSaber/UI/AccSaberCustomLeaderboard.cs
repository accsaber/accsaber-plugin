using System;
using AccSaber.Interfaces;
using AccSaber.Models;
using HMUI;
using LeaderboardCore.Managers;
using LeaderboardCore.Models;
using SiraUtil.Logging;

namespace AccSaber.UI
{
	internal sealed class AccSaberCustomLeaderboard : CustomLeaderboard, IDisposable, INotifyDifficultyBeatmapUpdated
	{
		private readonly SiraLog _log;
		private readonly CustomLeaderboardManager _customLeaderboardManager;

		public AccSaberCustomLeaderboard(SiraLog log, CustomLeaderboardManager customLeaderboardManager)
		{
			_log = log;
			_customLeaderboardManager = customLeaderboardManager;
		}

		protected override ViewController panelViewController { get; }
		protected override ViewController leaderboardViewController { get; }

		public void Dispose()
		{
			_customLeaderboardManager.Unregister(this);
		}

		public void DifficultyBeatmapUpdated(IDifficultyBeatmap difficultyBeatmap, AccSaberRankedMap? accSaberMapInfo)
		{
			_log.Info(accSaberMapInfo);
			return;
			
			if (accSaberMapInfo is not null)
			{
				_customLeaderboardManager.Register(this);
			}
			else
			{
				_customLeaderboardManager.Unregister(this);
			}
		}
	}
}