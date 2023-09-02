using AccSaber.Configuration;
using AccSaber.Managers;
using AccSaber.UI;
using AccSaber.UI.ViewControllers;
using AccSaber.Utils;
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

			Container.Bind<WebUtils>().AsSingle();
			Container.BindInterfacesAndSelfTo<AccSaberStore>().AsSingle();
			Container.BindInterfacesAndSelfTo<AccSaberManager>().AsSingle();
			Container.BindInterfacesTo<AccSaberCustomLeaderboard>().AsSingle();

			Container.BindInterfacesAndSelfTo<AccSaberPanelViewController>().FromNewComponentAsViewController().AsSingle();
		}
	}
}