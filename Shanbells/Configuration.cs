using Dalamud.Configuration;
using System;

namespace Shanbells;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public string ReplacementString { get; set; } = "Shanbells";

    public void Save()
    {
        Service.PluginInterface.SavePluginConfig(this);
    }
}