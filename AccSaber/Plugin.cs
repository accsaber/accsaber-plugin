using AccSaber.Installers;
using IPA;
using IPA.Loader;
using IPA.Logging;
using SiraUtil.Zenject;

namespace AccSaber
{
    [Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
    public class Plugin
    {
        public static Hive.Versioning.Version version;
        
        public static Logger _logger;

        [Init]
        public Plugin(Logger logger, PluginMetadata metadata, Zenjector zenjector)
        {
            version = metadata.HVersion;
            _logger = logger;
            
            zenjector.UseLogger(_logger);
            zenjector.Install<MenuInstaller>(Location.Menu);
        }
    }
}