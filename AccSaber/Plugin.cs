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

namespace AccSaber
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static Hive.Versioning.Version version;

        private readonly Harmony _harmony;
        private const string _harmonyID = "com.accsaber.plugin";
        private Logger _logger;

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
                HarmonyPatchPlaylistManager();
            }
        }

        private void HarmonyPatchPlaylistManager()
        {
            throw new NotImplementedException();
        }

        private bool IsPlaylistManagerInstalled()
        {
            try
            {
                PluginMetadata playlistManagerPlugin = PluginManager.EnabledPlugins.First(x => x.Id == "PlaylistManager");
                Hive.Versioning.Version invalidVersionRange = new Hive.Versioning.Version("1.5.0");
                return playlistManagerPlugin.HVersion < invalidVersionRange;
            }
            catch
            {
                return false;
            }
            
        }
    }
}