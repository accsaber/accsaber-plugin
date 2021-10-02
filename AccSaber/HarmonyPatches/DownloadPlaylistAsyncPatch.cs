using BeatSaberPlaylistsLib.Types;
using IPA.Utilities;
using PlaylistManager.UI;
using System;

namespace AccSaber.HarmonyPatches
{
    class DownloadPlaylistAsyncPatch
    {
        public static Action<PlaylistViewButtonsController> downloadMissingSongsCallback;

        public static bool Prefix(PlaylistViewButtonsController __instance)
        {
            var selectedPlaylist = __instance.GetField<Playlist, PlaylistViewButtonsController>("selectedPlaylist");
            if (!selectedPlaylist.TryGetCustomData("syncURL", out object outSyncURL))
            {
                return true;
            }

            string syncURL = (string)outSyncURL;
            if (IsAccSaberSyncURL(syncURL))
            {
                downloadMissingSongsCallback?.Invoke(__instance);
                return false;
            }

            return true;
        }

        private static bool IsAccSaberSyncURL(string syncURL)
        {
            return syncURL.Contains("api.accsaber.com/playlists/");
        }
    }
}
