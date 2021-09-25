//using BeatSaberPlaylistsLib.Types;
//using HarmonyLib;
//using IPA.Utilities;
//using PlaylistManager.UI;

//namespace AccSaber.HarmonyPatches
//{
//    [HarmonyPatch(typeof(PlaylistViewButtonsController), "DownloadPlaylistAsync")]
//    class DownloadPlaylistAsyncPatch
//    {
//        // Will probably switch this to transpiler
//        public static bool Prefix(PlaylistViewButtonsController __instance)
//        {
//            var selectedPlaylist = __instance.GetField<Playlist, PlaylistViewButtonsController>("selectedPlaylist");
//            if (!selectedPlaylist.TryGetCustomData("syncURL", out object outSyncURL))
//            {
//                return true;
//            }

//            string syncURL = (string)outSyncURL;
//            if (!IsAccSaberSyncURL(syncURL))
//            {
//                return true;
//            }

//            // AccSaber sync playlist, it's go time


//            return false;
//        }

//        private static bool IsAccSaberSyncURL(string syncURL)
//        {
//            return syncURL.Contains("api.accsaber.com/playlists/");
//        }
//    }
//}
