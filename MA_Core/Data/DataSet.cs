using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text.Json;
using MA_Core.Logic.StateMachines;
using MA_Core.Util;

[assembly: InternalsVisibleTo("MA_Test")]

namespace MA_Core.Data;

/// <summary>
/// A DataSet holds some <see cref="Action"/> and <see cref="Unit"/> data that can be edited and used for a battle
/// </summary>
public sealed class DataSet : IDisposable
{
    #region Properties

    #region constants

    private const string DatasetFileEnding = ".dataset";
    private const string DatasetJsonFileName = "DataSet.json";
    private const string MetadataJsonFileName = "Metadata.json";

    #endregion

    #region public
    
    // DataSet Properties
    public ValueObjects.FilePath Path { get; set; } = ValueObjects.FilePath.Empty;
    public string Name { get; set; } = String.Empty;
    public List<Action> Actions { get; } = [];
    public List<Unit> Units { get; } = [];
    
    // Metadata Properties
    /// MM.mmm (M = Major, m = minor)<br/>
    /// Major Versions are breaking, minor versions need minimal changes
    public double Version { get; private set; } = Versions.DataSetVersion;

    #endregion

    #region private

    private DirectoryInfo? UnpackedDirectory { get; set; }

    #endregion

    #endregion

    #region Init

    /// <summary>
    /// Creates an empty Dataset
    /// </summary>
    private DataSet()
    {
        
    }

    /// <summary>
    /// Deserializes a dataset from the given file
    /// </summary>
    /// <param name="path">Path to the dataset file</param>
    internal DataSet(ValueObjects.FilePath path)
    {
        if (!File.Exists((string)path)) throw new FileNotFoundException("File not found", (string)path);
        if (!((string)path).EndsWith(DatasetFileEnding)) throw new ArgumentException("Invalid file ending");

        using var zip = ZipFile.OpenRead((string)path);
        if (zip.Entries.None(x => x.Name.Equals(DatasetJsonFileName)))
            throw new FileNotFoundException($"Dataset does not contain '{DatasetJsonFileName}'", (string)path);
        if (zip.Entries.None(x => x.Name.Equals(MetadataJsonFileName)))
            throw new FileNotFoundException($"Dataset does not contain '{MetadataJsonFileName}'", (string)path);
        
        Path = path;
        UnpackedDirectory = TempDir.GetNewTempDir("dataset");
        
        var metaFile = zip.Entries.First(x => x.Name.Equals(MetadataJsonFileName));
        var metaJson = JsonDocument.Parse(metaFile.Open());
        var metaData = metaJson.Deserialize<DataSetMetadataJson>();
        if (metaData == null) throw new JsonException("File does not contain metadata");
        
        var datasetFile = zip.Entries.First(x => x.Name.Equals(DatasetJsonFileName));
        var datasetJson = JsonDocument.Parse(datasetFile.Open());
        var datasetData = datasetJson.Deserialize<DataSetJson>();
        if (datasetData == null) throw new JsonException("File does not contain dataset data");
        
        applyJsonConfig(datasetData, metaData);
        
        zip.ExtractToDirectory(UnpackedDirectory.FullName);
    }
    
    internal static DataSet Test_Factory(DataSetJson datasetData, DataSetMetadataJson metadataJson)
    {
        var dataSet = new DataSet();
        dataSet.applyJsonConfig(datasetData, metadataJson);
        return dataSet;
    }
    private void applyJsonConfig(DataSetJson datasetData, DataSetMetadataJson metadata)
    {
        if (Math.Abs(metadata.Version - Versions.DataSetVersion) > 0.0001)
        {
            throw new Exception($"DataSet Version mismatch. Expected: {Versions.DataSetVersion} - Actual: {metadata.Version}");
        }
        
        Version = metadata.Version;
        
        Name = datasetData.Name;
        
        Actions.Clear();
        Actions.AddRange(datasetData.Actions.Select(x => x.AsAction()).ToList());
        
        Units.Clear();
        Units.AddRange(datasetData.Units.Select(x => x.AsUnit(Actions)).ToList());
    }
    
    public void Dispose()
    {
        UnpackedDirectory!.Delete(true);
    }

    #endregion

    #region Methods

    public static ValueObjects.FilePath[] GetDataSetsFromFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath)) throw new DirectoryNotFoundException($"Directory not found: {folderPath}");
        var files = Directory.GetFiles(folderPath).Select(ValueObjects.FilePath.From);
        return files.Where(x => ((string)x).EndsWith(DatasetFileEnding)).ToArray();
    }

    public void Save(bool overwrite = false)
    {
        if (Path == ValueObjects.FilePath.Empty) throw new ArgumentException("Path is empty");
        if (!overwrite && File.Exists((string)Path)) throw new FileExistsException();
        
        try
        {
            var archiveFolder = TempDir.GetNewTempDir($"save_ds_{Name}");
            var resultFolder = TempDir.GetNewTempDir($"save_ds_archive_{Name}");

            #region Write Json

            // DataSet
            var dsJson = new DataSetJson(this);
            var dsJsonStr = JsonSerializer.Serialize(dsJson);
            var tmpPath = System.IO.Path.Join(archiveFolder.FullName, DatasetJsonFileName);
            File.WriteAllText(tmpPath, dsJsonStr);
            
            // Metadata
            var metaJson = new DataSetMetadataJson(this);
            var metaJsonStr = JsonSerializer.Serialize(metaJson);
            tmpPath = System.IO.Path.Join(archiveFolder.FullName, MetadataJsonFileName);
            File.WriteAllText(tmpPath, metaJsonStr);

            #endregion
            
            // Prepare Images
            
            // Bind Zip
            var archiveFileName = System.IO.Path.Join(resultFolder.FullName, "final.zip");
            ZipFile.CreateFromDirectory(archiveFolder.FullName, archiveFileName);
            File.Move(archiveFileName, (string)Path, true);
            
            // Cleanup Temp Folder
            archiveFolder.Delete(true);
            resultFolder.Delete(true);
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to save DataSet: {e.Message}", e);
        }
    }

    #endregion
}