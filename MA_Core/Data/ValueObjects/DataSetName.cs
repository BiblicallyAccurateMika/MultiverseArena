using Vogen;

namespace MA_Core.Data.ValueObjects;

[ValueObject<string>]
public partial struct DataSetName
{
    public static DataSetName Empty = new(String.Empty);
    
    private static string NormalizeInput(string input)
    {
        return input;
    }

    private static Validation Validate(string input)
    {
        if (String.IsNullOrWhiteSpace(input))
            return Validation.Invalid("Name cannot be empty");
        
        return Validation.Ok;
    }
}