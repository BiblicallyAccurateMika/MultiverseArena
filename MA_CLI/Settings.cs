using System.Text.Json;

namespace Ma_CLI;

public class Settings
{
    #region Const

    private const string SettingsFilePath = "settings.json";

    #endregion

    #region Singleton

    private static Settings? _instance;
    public static Settings Instance =>
        _instance ??= File.Exists(SettingsFilePath)
            ? JsonSerializer.Deserialize<Settings>(File.ReadAllText(SettingsFilePath))!
            : new Settings();

    #endregion

    #region Enums

    public enum ActionPlanViewType {Hybrid, OnlyData, OnlyDetail}

    #endregion

    #region Settings

    public string DataSetFolderPath { get; set; } = String.Empty;
    public ActionPlanViewType ActionPlanView { get; set; } = ActionPlanViewType.Hybrid;

    #endregion

    #region Methods

    public void Save()
    {
        File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize(this));
    }

    public void Edit(Action<Settings> action)
    {
        action(Instance);
        Save();
    }

    #endregion
}