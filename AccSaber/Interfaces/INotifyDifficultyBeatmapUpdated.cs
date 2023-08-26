using AccSaber.Models;

namespace AccSaber.Interfaces
{
	internal interface INotifyDifficultyBeatmapUpdated
	{
		public void DifficultyBeatmapUpdated(IDifficultyBeatmap difficultyBeatmap, AccSaberRankedMap? accSaberMapInfo);
	}
}