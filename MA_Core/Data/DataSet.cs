using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text.Json;

[assembly: InternalsVisibleTo("MA_Test")]

namespace MA_Core.Data;

/// <summary>
/// Stellt ein Dataset mit Einheiten und alle möglichen Aktionen dar
/// </summary>
public class DataSet
{
    #region Eigenschaften

    #region Constants

    private const string DatasetFileName = "DataSet.json";

    #endregion

    #region public
    
    /// Path for saving the dataset
    public string Path { get; private set; } = String.Empty;

    public List<Unit> Units { get; private set; } = [];
    public List<Action> Actions { get; private set; } = [];

    #endregion

    #endregion

    #region Initialisierung

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