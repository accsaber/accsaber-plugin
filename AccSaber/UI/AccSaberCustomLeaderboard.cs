using System;
using AccSaber.Interfaces;
using AccSaber.Models;
using AccSaber.UI.ViewControllers;
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
		private readonly AccSaberPanelViewController _accSaberPanelViewController;

		public AccSaberCustomLeaderboard(SiraLog log, CustomLeaderboardManager customLeaderboardManager, AccSaberPanelViewController accSaberPanelViewController)
		{
			_log = log;
			_customLeaderboardManager = customLeaderboardManager;
			_accSaberPanelViewController = accSaberPanelViewController;
		}

		protected override ViewController panelViewController { get; }
		protected override ViewController leaderboardViewController => _accSaberPanelViewController;

		public void Dispose()
		{
			_customLeaderboardManager.Unregister(this);
		}

		public void DifficultyBeatmapUpdated(IDifficultyBeatmap difficultyBeatmap, AccSaberRankedMap? accSaberMapInfo)
		{
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