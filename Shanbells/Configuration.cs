using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace Shanbells;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public string ReplacementString { get; set; } = "Shanbells";

    [NonSerialized]
    private IDalamudPluginInterface? _pluginInterface;

    public void Initialize(IDalamudPluginInterface pInterface)
    {
        _pluginInterface = pInterface;
    }

    public void Save()
    {
        _pluginInterface!.SavePluginConfig(this);
    }
}