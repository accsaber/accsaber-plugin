using System.Collections.Generic;
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

            zenjector.Install<CoreInstaller>(Location.App);
            zenjector.Install<MenuInstaller>(Location.Menu);
        }
    }
    
    public static class Extensions
    {
        public static string GetRankedSongHash(this string levelId) => !levelId.Contains("custom_level_") ? levelId : levelId.Substring(13);
        
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> tuple, out TKey key, out TValue value)
        {
            key = tuple.Key;
            value = tuple.Value;
        }
    }
}