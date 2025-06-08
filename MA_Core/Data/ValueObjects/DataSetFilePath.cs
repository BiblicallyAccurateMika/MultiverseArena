using Vogen;

namespace MA_Core.Data.ValueObjects;

[ValueObject<FilePath>]
public partial struct DataSetFilePath
{
    private const string DatasetFileEnding = ".dataset";
    
    public static readonly DataSetFilePath Empty = new(FilePath.Empty);
    
    private static Validation Validate(FilePath input)
    {
        var path = (string)input;

        return path.EndsWith(DatasetFileEnding) ? Validation.Ok : Validation.Invalid("Invalid file format");
    }
    
    // Makes it easier to convert from strings
    public static DataSetFilePath From(string input)
    {
        return From(FilePath.From(input));
    }
}