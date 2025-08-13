using System;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Shanbells;

public sealed class ShanbellsPlugin : IDalamudPlugin
{
    private string Name => "Shanbells";
    
    private const string CommandName = "/sbconfig";
    private readonly IDalamudPluginInterface _pluginInterface;
    private readonly Configuration _configuration;
    private readonly ConfigWindow _configWindow;
    private readonly WindowSystem _windowSystem = new("Shanbells");


    private const uint GilComponentNodeId = 18;
    private const uint GilTextNodeId = 2;
    private const uint GilTooltipTextNodeId = 2;
    private const uint GilItemDetailTextNodeId = 48;
    private const uint GilTalkTextNodeId = 3;

    public ShanbellsPlugin(IDalamudPluginInterface pluginInterface)
    {
        _pluginInterface = pluginInterface;
        Service.Initialize(pluginInterface);
        
        _configuration = _pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        _configuration.Initialize(_pluginInterface);
        
        _configWindow = new ConfigWindow(_configuration);
        _windowSystem.AddWindow(_configWindow);
        
        Service.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens the Shanbells configuration window."
        });

        _pluginInterface.UiBuilder.Draw += DrawUi;
        _pluginInterface.UiBuilder.OpenConfigUi += OpenConfigUi;

        Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "Currency", OnAddonSetup);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "Tooltip", OnAddonSetup);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "ItemDetail", OnAddonSetup);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "Talk", OnAddonSetup);
        Service.PluginLog.Information($"{Name} loaded.");
    }

    public void Dispose()
    {
        _windowSystem.RemoveAllWindows();
        _configWindow.Dispose();
        
        Service.CommandManager.RemoveHandler(CommandName);
        _pluginInterface.UiBuilder.Draw -= DrawUi;
        _pluginInterface.UiBuilder.OpenConfigUi -= OpenConfigUi;

        Service.AddonLifecycle.UnregisterListener(OnAddonSetup);
        Service.PluginLog.Information($"{Name} disposed.");
    }
    
    private void OnCommand(string command, string args) => OpenConfigUi();
    private void DrawUi() => _windowSystem.Draw();
    private void OpenConfigUi() => _configWindow.IsOpen = true;

    
    private unsafe void OnAddonSetup(AddonEvent type, AddonArgs args)
    {
        var addon = (AtkUnitBase*)args.Addon.Address;
        switch (args.AddonName)
        {
            case "Currency":
                ChangeGilInCurrencyWindow(addon);
                break;
            case "Tooltip":
                ChangeGilInTooltips(addon);
                break;
            case "ItemDetail":
                ChangeGilInItemDetails(addon);
                break;
            case "Talk":
                // I'm not comfortable with this one. Too much potential for wrong replacements.
                // And don't want to run a complex regex. I might go back to this in the future.
                // ChangeGilInDialog(addon);
                break;
        }
    }


    private unsafe void ChangeGilInCurrencyWindow(AtkUnitBase* addon)
    {
        if (addon == null) return;

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
                textNode->SetText(_configuration.ReplacementString);
        }
        catch (Exception ex)
        {
            Service.PluginLog.Error(ex, "Error occurred during UI modification in Currency addon.");
        }
    }

    private unsafe void ChangeGilInTooltips(AtkUnitBase* addon)
    {
        if (addon == null) return;

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
                lines.Payloads.Add(new TextPayload(_configuration.ReplacementString));
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

    private unsafe void ChangeGilInItemDetails(AtkUnitBase* addon)
    {
        if (addon == null) return;

        try
        {
            var textNode = addon->GetTextNodeById(GilItemDetailTextNodeId);
            if (textNode == null)
                return;
            
            string currentText = textNode->NodeText.ToString();

            if (currentText.Contains("gil"))
                textNode->SetText(currentText.Replace("gil", _configuration.ReplacementString, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            Service.PluginLog.Error(ex, "Error occurred during UI modification in Item Detail addon.");
        }
    }

    private unsafe void ChangeGilInDialog(AtkUnitBase* addon)
    {
        if (addon == null) return;

        try
        {
            var textNode = addon->GetTextNodeById(GilTalkTextNodeId);
            if (textNode == null)
                return;
            
            string currentText = textNode->NodeText.ToString();

            if (currentText.Contains("gil"))
                textNode->SetText(currentText.Replace("gil", _configuration.ReplacementString, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            Service.PluginLog.Error(ex, "Error occurred during UI modification in Talk addon.");
        }
    }
}