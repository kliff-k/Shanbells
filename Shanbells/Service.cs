using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

#pragma warning disable 8618
namespace Shanbells;

// ReSharper disable UnusedAutoPropertyAccessor.Local
internal class Service {
    internal static void Initialize(IDalamudPluginInterface pluginInterface) => pluginInterface.Create<Service>();

    [PluginService]
    internal static IDalamudPluginInterface PluginInterface { get; private set; }

    [PluginService]
    internal static ICommandManager CommandManager { get; private set; }

    [PluginService]
    internal static IPluginLog PluginLog { get; private set; }

    [PluginService]
    internal static IAddonLifecycle AddonLifecycle { get; private set; }
}
#pragma warning restore 8618