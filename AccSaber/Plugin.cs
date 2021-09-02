using AccSaber.Configuration;
using AccSaber.Installers;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Logging;
using SiraUtil.Zenject;

namespace AccSaber
{
	[Plugin(RuntimeOptions.DynamicInit)]
	public class Plugin
	{
		[Init]
		public void Init(Logger logger, Config config, Zenjector zenjector)
		{
			zenjector.OnMenu<MenuInstaller>();
		}

		[OnEnable, OnDisable]
		public void OnStateChanged()
		{
		}
	}
}