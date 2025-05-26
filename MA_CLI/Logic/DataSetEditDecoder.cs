using MA_Core.Data;
using MA_Core.Logic.Managers;

namespace Ma_CLI.Logic;

public static class DataSetEditDecoder
{
    public static bool ValidateEdit(DataSet dataSet, string key, out string error, params string[] args)
    {
        var valid = false;
        error = String.Empty;

        try
        {
            decodeEditCommand(dataSet, key, validate:true, args);
            valid = true;
        }
        catch (Exception e)
        {
            error = e.Message;
            valid = false;
        }
        
        return valid;
    }

    public static bool Edit(DataSet dataSet, string key, out string error, params string[] args)
    {
        var valid = false;
        error = String.Empty;

        try
        {
            decodeEditCommand(dataSet, key, validate:false, args);
            valid = true;
        }
        catch (Exception e)
        {
            error = e.Message;
            valid = false;
        }
        
        return valid;
    }

    private static void decodeEditCommand(DataSet dataSet, string key, bool validate, params string[] args)
    {
        ArgumentNullException.ThrowIfNull(dataSet);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(args);

        var execute = !validate;

        switch (key)
        {
            case "name":
                if (args.Length != 1)
                    throw new Exception("Invalid argument count");
                var name = args[0];
                DataSetManager.EditName(dataSet, name, execute);
                break;
            case "path":
                if (args.Length != 1)
                    throw new Exception("Invalid argument count");
                var path = args[0];
                DataSetManager.EditPath(dataSet, path, execute);
                break;
            default:
                throw new ArgumentException("Invalid key");
        }
    }
}