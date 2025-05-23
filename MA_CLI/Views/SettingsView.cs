﻿using System.Text;

namespace Ma_CLI.Views;

public class SettingsView(View parent) : View(parent)
{
    protected override string ViewName => "Settings";
    protected override Interaction[] Interactions => 
    [
        new("?", "Help", _ => action(() => _showHelp = true)),
        new("_", "Edit Settings", editSetting, validateCommand)
    ];

    private bool _showHelp;

    protected override void render(StringBuilder builder)
    {
        builder.AppendLine($"1 - DataSetFolderPath: {Settings.Instance.DataSetFolderPath}");
        builder.AppendLine($"2 - ActionPlanView: {Settings.Instance.ActionPlanView}");

        if (!_showHelp) return;
        builder.AppendLine();
        builder.AppendLine("<ID / Setting Name> <Value>");
    }

    // Quickhack, if there are more settings add them like below or make a proper logic for dynamically displaying and editing the settings
    private static bool validateCommand(string? command)
    {
        if (String.IsNullOrWhiteSpace(command)) return false;

        if (command.StartsWith("1 ") || command.StartsWith("DataSetFolderPath "))
        {
            var path = command.Replace("DataSetFolderPath ", "").Replace("1 ", "");
            if (Directory.Exists(path)) return true;
            throw new Exception("Path is invalid");
        }
        if (command.StartsWith("2 ") || command.StartsWith("ActionPlanView "))
        {
            var value = command.Replace("ActionPlanView ", "").Replace("2 ", "");
            if (value.Equals("0") || value.Equals("1") || value.Equals("2")) return true;
            throw new Exception("Value is invalid");
        }

        return false;
    }
    private static InteractionResult editSetting(string command)
    {
        if (command.StartsWith("1 ") || command.StartsWith("DataSetFolderPath "))
        {
            var path = command.Replace("DataSetFolderPath ", "").Replace("1 ", "");
            Settings.Instance.Edit(x => x.DataSetFolderPath = path);
            return done();
        }
        if (command.StartsWith("2 ") || command.StartsWith("ActionPlanView "))
        {
            var valueStr = command.Replace("ActionPlanView ", "").Replace("2 ", "");
            var value = (Settings.ActionPlanViewType)Int32.Parse(valueStr);
            Settings.Instance.Edit(x => x.ActionPlanView = value);
            return done();
        }

        throw new Exception("Unknown Setting");
    }
}