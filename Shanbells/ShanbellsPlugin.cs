using System;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Dalamud.Plugin.Services;

namespace Shanbells;

public sealed class ShanbellsPlugin : IDalamudPlugin
{
    public string Name => "Shanbells";

    private const uint GilComponentNodeId = 18;
    private const uint GilTextNodeId = 2;
    private const uint GilTooltipTextNodeId = 2;
    private const uint GilItemDetailTextNodeId = 48;

    public ShanbellsPlugin(IDalamudPluginInterface pluginInterface)
    {
        Service.Initialize(pluginInterface);
        Service.Framework.Update += FrameworkOnUpdate;
        Service.PluginLog.Information($"{Name} loaded.");
    }

    public void Dispose()
    {
        Service.Framework.Update -= FrameworkOnUpdate;
        Service.PluginLog.Information($"{Name} disposed.");
    }

    private unsafe void FrameworkOnUpdate(IFramework framework)
    {
        ChangeGilCurrencyName();
        ChangeGilTooltipName();
        ChangeGilItemDetailName();
    }

    private unsafe void ChangeGilCurrencyName()
    {
        var currencyAddon = AtkStage.Instance()->RaptureAtkUnitManager->GetAddonByName("Currency");
        
        if (currencyAddon == null || !currencyAddon->IsVisible)
            return;
        
        try
        {
            var componentNode = currencyAddon->GetComponentByNodeId(GilComponentNodeId);
            if (componentNode == null)
            {
                Service.PluginLog.Information($"Currency component node not found.");
                return;
            }
            
            var resourceNode = componentNode->GetTextNodeById(GilTextNodeId);
            if (resourceNode == null)
            {
                Service.PluginLog.Information($"Currency resource node not found.");
                return;
            }
            
            var textNode = resourceNode->GetAsAtkTextNode();
            if (textNode == null)
            {
                Service.PluginLog.Information($"Currency text node not found.");
                return;
            }
            
            string currentText = textNode->NodeText.ToString();
    
            if (currentText == "Gil")
                textNode->SetText("Shanbells");
        }
        catch (Exception ex)
        {
            Service.PluginLog.Error(ex, "Error occurred during UI modification in Currency addon.");
        }
    }

    private unsafe void ChangeGilTooltipName()
    {
        var tooltipAddon = AtkStage.Instance()->RaptureAtkUnitManager->GetAddonByName("Tooltip");
        
        if (tooltipAddon == null || !tooltipAddon->IsVisible)
            return;
        
        try
        {
            var textNode = tooltipAddon->GetTextNodeById(GilTooltipTextNodeId);
            if (textNode == null)
            {
                Service.PluginLog.Information($"Tooltip text node not found.");
                return;
            }
            
            string currentText = textNode->NodeText.ToString();

            if (currentText == "H�%I�&GilIH")
            {
                var lines = new SeString();
                lines.Payloads.Add(new UIForegroundPayload(549));
                lines.Payloads.Add(new UIGlowPayload(550));
                lines.Payloads.Add(new TextPayload("Shanbells"));
                lines.Payloads.Add(new UIGlowPayload(0));
                lines.Payloads.Add(new UIForegroundPayload(0));
                textNode->SetText(lines.Encode());
                textNode->ResizeNodeForCurrentText();
                textNode->NextSiblingNode->SetWidth(85);
            }
        }
        catch (Exception ex)
        {
            Service.PluginLog.Error(ex, "Error occurred during UI modification in Tooltip addon.");
        }
    }

    private unsafe void ChangeGilItemDetailName()
    {
        var tooltipAddon = AtkStage.Instance()->RaptureAtkUnitManager->GetAddonByName("ItemDetail");
        
        if (tooltipAddon == null || !tooltipAddon->IsVisible)
            return;
        
        try
        {
            var textNode = tooltipAddon->GetTextNodeById(GilItemDetailTextNodeId);
            if (textNode == null)
            {
                Service.PluginLog.Information($"Item Detail text node not found.");
                return;
            }
            
            string currentText = textNode->NodeText.ToString();

            if (currentText.Contains("gil"))
            {
                textNode->SetText(currentText.Replace("gil", "shanbells"));
            }
        }
        catch (Exception ex)
        {
            Service.PluginLog.Error(ex, "Error occurred during UI modification in Item Detail addon.");
        }
    }
}