using AccSaber.Configuration;
using AccSaber.Installers;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace AccSaber
{
	[Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
	public class Plugin
	{
        [Init]
		public void Init(Zenjector zenjector, IPALogger logger, Config config)
		{
			zenjector.UseLogger(logger);
			zenjector.UseHttpService();
            
			zenjector.Install<AccSaberMenuInstaller>(Location.Menu, config.Generated<PluginConfig>());
		}
	}
}