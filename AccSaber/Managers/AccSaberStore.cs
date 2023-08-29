using System;
using System.Collections.Generic;
using AccSaber.Models;
using SiraUtil.Logging;
using Zenject;

namespace AccSaber.Managers
{
	internal sealed class AccSaberStore : IInitializable
	{
		private readonly SiraLog _log;
		private readonly Downloader _downloader;
		
		public event Action<AccSaberRankedMap?>? OnAccSaberRankedMapUpdated; 

		public readonly Dictionary<string, AccSaberRankedMap> RankedMaps = new();
		private AccSaberRankedMap? _currentRankedMap = null;

		public AccSaberStore(SiraLog log, Downloader downloader)
		{
			_log = log;
			_downloader = downloader;
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
				RankedMaps[$"{map.songHash}/{map.difficulty}".ToLower()] = map;
			}
		}
	}
}