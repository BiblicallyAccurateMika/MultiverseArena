using MA_Core.Logic.Objects;

namespace Ma_CLI;

public class Program
{
    public static void Main(string[] args)
    {
        Console.Write("Dataset: ");
        var path = Console.ReadLine();
        var dataSet = new DataSet(path);
    }
}
