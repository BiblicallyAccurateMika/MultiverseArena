using MA_Core.Data;
using Vogen;

namespace MA_Test.Tests.ValueObjects;

public class FilePathTests
{
    [SetUp]
    public void SetUp()
    {
        
    }

    /// Tests if the path gets normalized correctly
    [Test]
    public void TestNormalization()
    {
        var path = FilePath.From("\"C:\\test\\path\"");
        Assert.That((string)path, Is.EqualTo("C:/test/path"));
    }
    
    /// Tests if the validation works as intended
    [Test]
    public void TestValidation()
    {
        string[] pathStrings = [
            "C:/valid/path",
            "/unix/path",
            "C:/directory/path/",
            "C:/invalid?path"
        ];
        
        Assert.DoesNotThrow(() => _ = FilePath.From(pathStrings[0]));
        Assert.DoesNotThrow(() => _ = FilePath.From(pathStrings[1]));
        Assert.Throws<ValueObjectValidationException>(() => _ = FilePath.From(pathStrings[2]));
        Assert.Throws<ValueObjectValidationException>(() => _ = FilePath.From(pathStrings[3]));
    }
}