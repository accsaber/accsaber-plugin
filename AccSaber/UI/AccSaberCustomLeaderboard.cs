using System;
using AccSaber.Managers;
using AccSaber.Models;
using AccSaber.UI.ViewControllers;
using HMUI;
using LeaderboardCore.Managers;
using LeaderboardCore.Models;
using SiraUtil.Logging;
using Zenject;

namespace AccSaber.UI
{
	internal sealed class AccSaberCustomLeaderboard : CustomLeaderboard, IInitializable, IDisposable
	{
		private readonly SiraLog _log;
		private readonly AccSaberStore _accSaberStore;
		private readonly CustomLeaderboardManager _customLeaderboardManager;
		private readonly AccSaberPanelViewController _accSaberPanelViewController;

		public AccSaberCustomLeaderboard(SiraLog log, AccSaberStore accSaberStore, CustomLeaderboardManager customLeaderboardManager, AccSaberPanelViewController accSaberPanelViewController)
		{
			_log = log;
			_accSaberStore = accSaberStore;
			_customLeaderboardManager = customLeaderboardManager;
			_accSaberPanelViewController = accSaberPanelViewController;
		}

		protected override ViewController panelViewController { get; }
		protected override ViewController leaderboardViewController => _accSaberPanelViewController;

		public void Initialize()
		{
			_accSaberStore.OnAccSaberRankedMapUpdated += AccSaberStoreOnOnAccSaberRankedMapUpdated; 
		}

		public void Dispose()
		{
			_customLeaderboardManager.Unregister(this);
		}

		private void AccSaberStoreOnOnAccSaberRankedMapUpdated(AccSaberRankedMap? accSaberMapInfo)
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