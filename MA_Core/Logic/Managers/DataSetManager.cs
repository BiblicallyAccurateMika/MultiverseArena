using MA_Core.Data;

namespace MA_Core.Logic.Managers;

public static class DataSetManager
{
    public static bool EditPath(DataSet dataSet, string newPath, bool execute = false)
    {
        ArgumentNullException.ThrowIfNull(dataSet);
        ArgumentException.ThrowIfNullOrWhiteSpace(newPath);

        bool result = false;

        try
        {
            var dir = Path.GetDirectoryName(newPath);
            result = Directory.Exists(dir);

            if (!execute) return result;

            dataSet.Path = newPath;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public static bool EditName(DataSet dataSet, string newName, bool execute = false)
    {
        ArgumentNullException.ThrowIfNull(dataSet);
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);

        bool result = true;

        try
        {
            if (!execute) return result;

            dataSet.Name = newName;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private static void validateEdit(DataSet dataSet, string key, string[] args)
    {
        ArgumentNullException.ThrowIfNull(dataSet);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(args);

        switch (key)
        {
            case "name":
                if (args.Length != 1) throw new ArgumentException("Invalid argument count");
                break;
            case "path":
                if (args.Length != 1) throw new ArgumentException("Invalid argument count");
                var dir = Path.GetDirectoryName(args[0]);
                if (!Directory.Exists(dir)) throw new DirectoryNotFoundException();
                break;
            default:
                throw new ArgumentException("Unknown key");
        }
    }

    public static void ExecuteEdit(DataSet dataSet, string key, string[] args)
    {
        validateEdit(dataSet, key, args);
        
        switch (key)
        {
            case "name":
                dataSet.Name = args[0];
                break;
            case "path":
                dataSet.Path = args[0];
                break;
        }
    }
}