using Ma_CLI.Views;
using MA_Core.Data;
using MA_Core.Data.ValueObjects;
using MA_Core.Util;

namespace Ma_CLI.Logic;

public static class DataSetEditDecoder
{
    public enum EditKey
    {
        Path,
        Name,
        UnitAdd,
        UnitRemove
    }

    public static EditKey GetEditKeyFromCommand(View view, string command)
    {
        switch (command)
        {
            case "path":
                if (view is DataSetEditorView) return EditKey.Path;
                break;
            case "name":
                if (view is DataSetEditorView) return EditKey.Name;
                break;
            case "add":
                if (view is DataSetEditorView.UnitsView) return EditKey.UnitAdd;
                break;
            case "remove":
                if (view is DataSetEditorView.UnitsView) return EditKey.UnitRemove;
                break;
        }
        
        throw new Exception("Invalid command");
    }
    
    public static bool ValidateEdit(DataSet dataSet, View view, string command, out string error, params string[] args)
    {
        var valid = false;
        error = String.Empty;

        try
        {
            var key = GetEditKeyFromCommand(view, command);
            decodeEditCommand(dataSet, key, execute:false, args);
            valid = true;
        }
        catch (Exception e)
        {
            error = e.Message;
            valid = false;
        }
        
        return valid;
    }

    public static bool Edit(DataSet dataSet, View view, string command, out string error, params string[] args)
    {
        var valid = false;
        error = String.Empty;

        try
        {
            var key = GetEditKeyFromCommand(view, command);
            decodeEditCommand(dataSet, key, execute:true, args);
            valid = true;
        }
        catch (Exception e)
        {
            error = e.Message;
            valid = false;
        }
        
        return valid;
    }

    private static void decodeEditCommand(DataSet dataSet, EditKey key, bool execute, params string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        switch (key)
        {
            case EditKey.Path:
                if (args.Length != 1) throw new Exception("Invalid argument count");
                var path = DataSetFilePath.From(args[0]);
                if (execute) dataSet.Path = path;
                break;
            case EditKey.Name:
                if (args.Length != 1) throw new Exception("Invalid argument count");
                var name = DataSetName.From(args[0]);
                if (execute) dataSet.Name = name;
                break;
            case EditKey.UnitAdd:
                if (args.Length != 1) throw new Exception("Invalid argument count");
                if (dataSet.Units.Any(unit => unit.Codename == args[0])) throw new Exception("Codename already exists");
                if (execute) dataSet.Units.Add(new Unit(UnitCodeName.From(args[0])));
                break;
            case EditKey.UnitRemove:
                if (args.Length != 1) throw new Exception("Invalid argument count");
                if (dataSet.Units.None(unit => unit.Codename == args[0])) throw new Exception("Codename does not exists");
                if (execute) dataSet.Units.Remove(dataSet.Units.First(unit => unit.Codename == args[0]));
                break;
            default:
                throw new NotImplementedException();
        }
    }
}