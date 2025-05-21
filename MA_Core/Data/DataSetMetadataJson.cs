namespace MA_Core.Data;

public class DataSetMetadataJson
{
    public double Version { get; set; }

    public DataSetMetadataJson() { }
    public DataSetMetadataJson(DataSet dataSet)
    {
        Version = dataSet.Version;
    }
}