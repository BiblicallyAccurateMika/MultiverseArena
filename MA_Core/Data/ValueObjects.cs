using Vogen;

namespace MA_Core.Data;

[ValueObject<string>]
public partial struct FilePath
{
    public static readonly FilePath Empty = new(String.Empty);

    private static string NormalizeInput(string input)
    {
        input = input.Trim();
        input = input.Replace("\\", "/");
        if (input.Contains('\"'))
        {
            input = input.Replace("\"", "");
        }
        return input;
    }
    
    private static Validation Validate(string input)
    {
        if (String.IsNullOrEmpty(input)) return Validation.Invalid("String is empty");
        
        try
        {
            var file = Path.GetFileName(input);
            return String.IsNullOrEmpty(file) ? Validation.Invalid("Path is not a file") : Validation.Ok;
        }
        catch (ArgumentException)
        {
            return Validation.Invalid("Invalid Path");
        }
    }
}