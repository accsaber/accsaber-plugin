using AccSaber.Installers;
using IPA;
using IPA.Config;
using IPA.Logging;
using SiraUtil;
using SiraUtil.Zenject;

namespace AccSaber
{
	[Plugin(RuntimeOptions.DynamicInit)]
	public class Plugin
	{
		[Init]
		public void Init(Logger logger, Config config, Zenjector zenjector)
		{
			zenjector.On<PCAppInit>().Pseudo(Container =>
				Container.BindLoggerAsSiraLogger(logger));
			zenjector.OnMenu<MenuInstaller>();
		}
	}
}	