using AccSaber.Installers;
using IPA;
using IPA.Config;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace AccSaber
{
	[Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
	public class Plugin
	{
		internal static Hive.Versioning.Version HVersion;
		
        [Init]
		public void Init(Zenjector zenjector, IPALogger logger, Config config)
		{
			zenjector.UseLogger(logger);
			zenjector.UseHttpService();
			
			zenjector.Install<AccSaberAppInstaller>(Location.App, config);
			zenjector.Install<AccSaberMenuInstaller>(Location.Menu);
		}
	}
}