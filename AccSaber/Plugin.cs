using AccSaber.HarmonyPatches;
using AccSaber.Installers;
using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Loader;
using IPA.Logging;
using SiraUtil;
using SiraUtil.Zenject;
using System;
using System.Linq;
using System.Reflection;

namespace AccSaber
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static Hive.Versioning.Version version;

        private readonly Harmony _harmony;
        private const string _harmonyID = "com.accsaber.plugin";
        public static Logger _logger;
        internal static bool playlistManagerPatching = false;

        [Init]
        public Plugin(Logger logger, Config config, PluginMetadata metadata, Zenjector zenjector)
        {
            version = metadata.HVersion;
            _logger = logger;
            _harmony = new Harmony(_harmonyID);

            zenjector.On<PCAppInit>().Pseudo(Container => Container.BindLoggerAsSiraLogger(logger));
            zenjector.OnMenu<MenuInstaller>();
        }

        [OnEnable]
        public void OnEnable()
        {
            PatchPlaylistManager();
        }

        [OnDisable]
        public void OnDisable()
        {
            _harmony.UnpatchAll(_harmonyID);
        }

        private void PatchPlaylistManager()
        {
            if (IsPlaylistManagerInstalled())
            {
                _logger.Debug("PlaylistManager installed, using Harmony patches");
                playlistManagerPatching = true;
                HarmonyPatchPlaylistManager();
            }
            else
            {
                _logger.Debug("No playlist manager");
            }
        }

        private void HarmonyPatchPlaylistManager()
        {
            // DownloadPlaylistAsync
            var originalDownloadPlaylistAsync = typeof(PlaylistManager.UI.PlaylistViewButtonsController).GetMethod("DownloadPlaylistAsync", (BindingFlags)(-1));
            HarmonyMethod harmonyDownloadPlaylistAsyncPrefix = new HarmonyMethod(typeof(DownloadPlaylistAsyncPatch).GetMethod("Prefix", (BindingFlags)(-1)));
            _harmony.Patch(originalDownloadPlaylistAsync, harmonyDownloadPlaylistAsyncPrefix);

            // UpdateSecondChildControllerContent
            var originalUpdateSecondChildControllerContent = typeof(LevelFilteringNavigationController).GetMethod("UpdateSecondChildControllerContent", (BindingFlags)(-1));
            HarmonyMethod harmonyUpdateSecondChildControllerContentPostfix = new HarmonyMethod(typeof(UpdateSecondChildControllerContentPatch).GetMethod("Postfix", (BindingFlags)(-1)));
            _harmony.Patch(originalUpdateSecondChildControllerContent, null, harmonyUpdateSecondChildControllerContentPostfix);
        }

        private bool IsPlaylistManagerInstalled()
        {
            try
            {
                PluginMetadata playlistManagerPlugin = PluginManager.EnabledPlugins.First(x => x.Id == "PlaylistManager");
                Hive.Versioning.Version invalidVersionRange = new Hive.Versioning.Version("1.6.0");
                return playlistManagerPlugin.HVersion < invalidVersionRange;
            }
            catch (Exception e)
            {
                _logger.Debug($"{e.Message}");
                return false;
            }

        }
    }
}