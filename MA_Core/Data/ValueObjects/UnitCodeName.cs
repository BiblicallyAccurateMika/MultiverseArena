using Vogen;

namespace MA_Core.Data.ValueObjects;

[ValueObject<string>]
public partial struct UnitCodeName
{
    public static UnitCodeName Empty = new(String.Empty);
    
    private static string NormalizeInput(string input)
    {
        return input;
    }

    private static Validation Validate(string input)
    {
        if (String.IsNullOrWhiteSpace(input))
            return Validation.Invalid("CodeName can not be empty");
        
        return Validation.Ok;
    }
}