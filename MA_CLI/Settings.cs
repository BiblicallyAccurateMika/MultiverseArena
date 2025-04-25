using System.Text.Json;

namespace Ma_CLI;

public class Settings
{
    #region Const

    private const string SettingsFilePath = "settings.json";

    #endregion

    #region Singleton

    private static Settings? _instance;
    public static Settings Instance => _instance ??= JsonSerializer.Deserialize<Settings>(File.ReadAllText(SettingsFilePath))!;

    private Settings() { }

    #endregion

    #region Settings

    public string DataSetFolderPath { get; init; } = null!;

    #endregion

    #region Methods

    public void Save()
    {
        File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize(this));
    }

    #endregion
}