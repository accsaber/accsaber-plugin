using AccSaber.Configuration;
using AccSaber.Interfaces;
using AccSaber.Managers;
using AccSaber.UI;
using Zenject;

namespace AccSaber.Installers
{
	internal sealed class AccSaberMenuInstaller : Installer
	{
		private readonly PluginConfig _pluginConfig;

		public AccSaberMenuInstaller(PluginConfig pluginConfig)
		{
			_pluginConfig = pluginConfig;
		}
		
		public override void InstallBindings()
		{
			Container.BindInstance(_pluginConfig).AsSingle();

			Container.Bind<Downloader>().AsSingle();
			Container.BindInterfacesAndSelfTo<AccSaberManager>().AsSingle();
			Container.BindInterfacesTo<AccSaberCustomLeaderboard>().AsSingle();
		}
	}
}