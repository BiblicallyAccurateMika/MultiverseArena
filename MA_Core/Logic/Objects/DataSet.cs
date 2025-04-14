using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text.Json;
using MA_Core.Data;

[assembly: InternalsVisibleTo("MA_Test")]

namespace MA_Core.Logic.Objects;

/// <summary>
/// Stellt ein Dataset mit Einheiten und alle möglichen Aktionen dar
/// </summary>
public class DataSet
{
    #region Eigenschaften
    
    private const string DatasetFileName = "DataSet.json";

    /// <summary>
    /// Path for saving the dataset
    /// </summary>
    public string Path { get; set; } = String.Empty;

    public List<Data.Unit> Units { get; set; } = [];
    public List<Data.Action> Actions { get; set; } = [];

    #endregion

    #region Initialisierung

    /// <summary>
    /// Creates a empty Dataset
    /// </summary>
    public DataSet()
    {
        
    }

    /// <summary>
    /// Deserializes a dataset from the given file
    /// </summary>
    /// <param name="path">Path to the dataset file</param>
    public DataSet(string path)
    {
        if (String.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found", path);
        }
        
        var zip = ZipFile.OpenRead(path);

        if (!zip.Entries.Any(x => x.Name.Equals(DatasetFileName)))
        {
            throw new FileNotFoundException($"Dataset does not contain '{DatasetFileName}'", path);
        }
        
        Path = path;
        
        var file = zip.Entries.First(x => x.Name.Equals(DatasetFileName));

        var json = JsonDocument.Parse(file.Open());
        var datasetData = json.Deserialize<DataSetJson>();

        if (datasetData == null)
        {
            throw new JsonException("File does not contain dataset data");
        }
        
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
        Actions = datasetData.Actions.Select(x => x.AsAction()).ToList();
        Units = datasetData.Units.Select(x => x.AsUnit(Actions)).ToList();
    }

    #endregion

    #region Methoden

    #endregion
}