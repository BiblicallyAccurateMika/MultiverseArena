using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text.Json;

[assembly: InternalsVisibleTo("MA_Test")]

namespace MA_Core.Data;

/// <summary>
/// A DataSet holds some <see cref="Action"/> and <see cref="Unit"/> data that can be edited and used for a battle
/// </summary>
public class DataSet
{
    #region Properties

    #region constants

    public const string DatasetFileEnding = ".dataset";
    private const string DatasetJsonFileName = "DataSet.json";

    #endregion

    #region public
    
    /// Path for saving the dataset
    public string Path { get; private set; } = String.Empty;

    public string Name { get; set; } = String.Empty;
    public List<Action> Actions { get; private set; } = [];
    public List<Unit> Units { get; private set; } = [];

    #endregion

    #endregion

    #region Init

    /// <summary>
    /// Creates a empty Dataset
    /// </summary>
    private DataSet()
    {
        
    }

    /// <summary>
    /// Deserializes a dataset from the given file
    /// </summary>
    /// <param name="path">Path to the dataset file</param>
    internal DataSet(string path)
    {
        if (String.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        if (path.StartsWith('"') && path.EndsWith('"')) path = path[1..^1]; // Removes quotation marks
        if (!File.Exists(path)) throw new FileNotFoundException("File not found", path);
        if (!path.EndsWith(DatasetFileEnding)) throw new ArgumentException("Invalid file ending");

        using var zip = ZipFile.OpenRead(path);
        if (!zip.Entries.Any(x => x.Name.Equals(DatasetJsonFileName)))
            throw new FileNotFoundException($"Dataset does not contain '{DatasetJsonFileName}'", path);
        
        Path = path;
        
        var file = zip.Entries.First(x => x.Name.Equals(DatasetJsonFileName));

        var json = JsonDocument.Parse(file.Open());
        var datasetData = json.Deserialize<DataSetJson>();

        if (datasetData == null) throw new JsonException("File does not contain dataset data");
        
        applyJsonConfig(datasetData);
    }
    
    internal static DataSet Test_Factory(DataSetJson datasetData)
    {
        var dataSet = new DataSet();
        dataSet.applyJsonConfig(datasetData);
        return dataSet;
    }
    private void applyJsonConfig(DataSetJson datasetData)
    {
        Name = datasetData.Name;
        Actions = datasetData.Actions.Select(x => x.AsAction()).ToList();
        Units = datasetData.Units.Select(x => x.AsUnit(Actions)).ToList();
    }

    #endregion

    #region Methods

    public static string[] GetDataSetsFromFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath)) throw new DirectoryNotFoundException($"Directory not found: {folderPath}");
        var files = Directory.GetFiles(folderPath);
        return files.Where(x => x.EndsWith(DatasetFileEnding)).ToArray();
    }

    public void Save()
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(Path, nameof(Path));

            var temp = Directory.CreateTempSubdirectory("multiverseArena_");
            var tempFolder = temp.CreateSubdirectory("dataset").FullName;

            // Write Json
            var dsJson = new DataSetJson(this);
            var dsJsonStr = JsonSerializer.Serialize(dsJson);
            var tmpPath = System.IO.Path.Join(tempFolder, DatasetJsonFileName);
            File.WriteAllText(tmpPath, dsJsonStr);

            // Prepare Images

            // Bind Zip
            var archiveFileName = System.IO.Path.Join(temp.FullName, "final.zip");
            ZipFile.CreateFromDirectory(tempFolder, archiveFileName);
            File.Move(archiveFileName, Path, true);
            
            // Cleanup Temp Folder
            temp.Delete(true);
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to save DataSet: {e.Message}", e);
        }
    }

    #endregion
}