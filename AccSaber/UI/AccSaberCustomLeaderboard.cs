using System;
using AccSaber.Managers;
using AccSaber.Models;
using AccSaber.UI.ViewControllers;
using HMUI;
using LeaderboardCore.Managers;
using LeaderboardCore.Models;
using Zenject;

namespace AccSaber.UI
{
	internal sealed class AccSaberCustomLeaderboard : CustomLeaderboard, IInitializable, IDisposable
	{
		private readonly AccSaberStore _accSaberStore;
		private readonly CustomLeaderboardManager _customLeaderboardManager;
		private readonly AccSaberPanelViewController _accSaberPanelViewController;
		private readonly AccSaberLeaderboardViewController _accSaberLeaderboardViewController;

		public AccSaberCustomLeaderboard(AccSaberStore accSaberStore, CustomLeaderboardManager customLeaderboardManager, AccSaberPanelViewController accSaberPanelViewController, AccSaberLeaderboardViewController accSaberLeaderboardViewController)
		{
			_accSaberStore = accSaberStore;
			_customLeaderboardManager = customLeaderboardManager;
			_accSaberPanelViewController = accSaberPanelViewController;
			_accSaberLeaderboardViewController = accSaberLeaderboardViewController;
		}

		protected override ViewController panelViewController => _accSaberPanelViewController;
		protected override ViewController leaderboardViewController => _accSaberLeaderboardViewController;

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