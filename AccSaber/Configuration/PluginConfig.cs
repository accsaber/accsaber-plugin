using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace AccSaber.Configuration
{
	internal class PluginConfig
	{
		public virtual bool RainbowHeader { get; set; } = false;
	}
}