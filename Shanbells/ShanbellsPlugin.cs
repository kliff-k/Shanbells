using System;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Dalamud.Plugin.Services;

namespace Shanbells;

public sealed class ShanbellsPlugin : IDalamudPlugin
{
    private string Name => "Shanbells";

    private const uint GilComponentNodeId = 18;
    private const uint GilTextNodeId = 2;
    private const uint GilTooltipTextNodeId = 2;
    private const uint GilItemDetailTextNodeId = 48;
    private const uint GilTalkTextNodeId = 3;

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

    private void FrameworkOnUpdate(IFramework framework)
    {
        ChangeGilInCurrencyWindow();
        ChangeGilInTooltips();
        ChangeGilInItemDetails();
        ChangeGilInDialog();
    }

    private unsafe void ChangeGilInCurrencyWindow()
    {
        var addon = AtkStage.Instance()->RaptureAtkUnitManager->GetAddonByName("Currency");
        
        if (addon == null || !addon->IsVisible)
            return;
        
        try
        {
            var componentNode = addon->GetComponentByNodeId(GilComponentNodeId);
            if (componentNode == null)
                return;
            
            var resourceNode = componentNode->GetTextNodeById(GilTextNodeId);
            if (resourceNode == null)
                return;
            
            var textNode = resourceNode->GetAsAtkTextNode();
            if (textNode == null)
                return;
            
            string currentText = textNode->NodeText.ToString();
    
            if (currentText == "Gil")
                textNode->SetText("Shanbells");
        }
        catch (Exception ex)
        {
            Service.PluginLog.Error(ex, "Error occurred during UI modification in Currency addon.");
        }
    }

    private unsafe void ChangeGilInTooltips()
    {
        var addon = AtkStage.Instance()->RaptureAtkUnitManager->GetAddonByName("Tooltip");
        
        if (addon == null || !addon->IsVisible)
            return;
        
        try
        {
            var textNode = addon->GetTextNodeById(GilTooltipTextNodeId);
            if (textNode == null)
                return;
            
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

    private unsafe void ChangeGilInItemDetails()
    {
        var addon = AtkStage.Instance()->RaptureAtkUnitManager->GetAddonByName("ItemDetail");
        
        if (addon == null || !addon->IsVisible)
            return;
        
        try
        {
            var textNode = addon->GetTextNodeById(GilItemDetailTextNodeId);
            if (textNode == null)
                return;
            
            string currentText = textNode->NodeText.ToString();

            if (currentText.Contains("gil"))
                textNode->SetText(currentText.Replace("gil", "shanbells"));
        }
        catch (Exception ex)
        {
            Service.PluginLog.Error(ex, "Error occurred during UI modification in Item Detail addon.");
        }
    }

    private unsafe void ChangeGilInDialog()
    {
        var addon = AtkStage.Instance()->RaptureAtkUnitManager->GetAddonByName("Talk");
        
        if (addon == null || !addon->IsVisible)
            return;
        
        try
        {
            var textNode = addon->GetTextNodeById(GilTalkTextNodeId);
            if (textNode == null)
                return;
            
            string currentText = textNode->NodeText.ToString();

            if (currentText.Contains("gil"))
                textNode->SetText(currentText.Replace("gil", "shanbells"));
        }
        catch (Exception ex)
        {
            Service.PluginLog.Error(ex, "Error occurred during UI modification in Talk addon.");
        }
    }
}