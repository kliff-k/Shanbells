using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace Shanbells;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration _configuration;

    public ConfigWindow(Configuration configuration) : base("Shanbells Configuration")
    {
        _configuration = configuration;
        Size = new Vector2(232, 75);
        SizeCondition = ImGuiCond.Once;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var replacementString = _configuration.ReplacementString;
        if (ImGui.InputText("Replacement Word", ref replacementString, 100))
        {
            _configuration.ReplacementString = replacementString;
            _configuration.Save();
        }
    }
}