using MA_Core.Data;

namespace MA_Core.Logic.Managers;

public static class DataSetManager
{
    public static bool ValidateEdit(DataSet dataSet, string key, string[] args)
    {
        ArgumentNullException.ThrowIfNull(dataSet);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(args);

        if (key.Equals("name") && args.Length == 1)
        {
            return true;
        }
        else if (key.StartsWith("actions."))
        {
            
        }
        else if (key.StartsWith("units."))
        {
            
        }

        throw new ArgumentException("Unknown key");
    }

    public static bool ExecuteEdit(DataSet dataSet, string key, string[] args)
    {
        if (key.Equals("name")) dataSet.Name = args[0];
        
        return false;
    }

    public static bool ValidateAndExecuteEdit(DataSet dataSet, string key, string[] args)
    {
        return ValidateEdit(dataSet, key, args) && ExecuteEdit(dataSet, key, args);
    }
}