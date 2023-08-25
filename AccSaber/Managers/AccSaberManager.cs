using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccSaber.Models;
using SiraUtil.Logging;
using Zenject;

namespace AccSaber.Managers
{
	internal class AccSaberManager : IInitializable, IDisposable
	{
		private readonly SiraLog _log;
		private Downloader _downloader;
		private UserIDUtils _userID;

		private string UserId;
		public AccSaberManager(Downloader downloader, UserIDUtils userID)
		{
			_downloader = downloader;
			_userID = userID;
		}

		public void Initialize()
		{
			UserId = _userID.GetUserID().ConfigureAwait(false).ToString();
		}

		private async Task GetLeaderboardData()
		{
			var userData = await _downloader.Get<List<AccSaberLeaderboardEntry>>("https://api.accsaber.com/ranked-maps/");
			foreach (var data in userData)
			{
				_log.Info(data);
			}
		}

		public void Dispose()
		{
		}
	}
}