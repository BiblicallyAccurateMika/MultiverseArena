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
            if (input.Any(x => Path.GetInvalidPathChars().Contains(x))) return Validation.Invalid("String contains invalid path characters");
            
            var file = Path.GetFileName(input);
            
            if (String.IsNullOrEmpty(file)) return Validation.Invalid("Path is not a file");
            if (file.Any(x => Path.GetInvalidFileNameChars().Contains(x))) return Validation.Invalid("String contains invalid filename characters");
        }
        catch (ArgumentException)
        {
            return Validation.Invalid("Invalid Path");
        }
        
        return Validation.Ok;
    }
}