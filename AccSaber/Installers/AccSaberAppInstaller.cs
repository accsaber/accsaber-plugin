﻿using AccSaber.Configuration;
using Zenject;

namespace AccSaber.Installers
{
	internal sealed class AccSaberAppInstaller : Installer
	{
		private readonly PluginConfig _pluginConfig;

		public AccSaberAppInstaller(PluginConfig pluginConfig)
		{
			_pluginConfig = pluginConfig;
		}

		public override void InstallBindings()
		{
			Container.BindInstance(_pluginConfig).AsSingle();
			Container.Bind<UserIDUtils>().AsSingle();
		}
	}
}