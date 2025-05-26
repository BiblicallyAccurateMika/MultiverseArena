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
            result = true;
        }
        catch (Exception ex)
        {
            result = false;
        }
        
        return result;
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
            result = true;
        }
        catch (Exception ex)
        {
            result = false;
        }
        
        return result;
    }
}