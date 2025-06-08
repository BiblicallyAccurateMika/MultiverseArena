using MA_Core.Data;
using MA_Core.Util;

namespace MA_Core.Logic.Managers;

public static class DataSetManager
{
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

    public static bool AddUnit(DataSet dataSet, string newUnitCodename, bool execute = false)
    {
        ArgumentNullException.ThrowIfNull(dataSet);
        ArgumentException.ThrowIfNullOrWhiteSpace(newUnitCodename);

        bool result;

        try
        {
            // We can only add a new unit if the codename isn't used already
            result = dataSet.Units.None(x => x.Codename == newUnitCodename);
            
            if (!execute) return result;
            if (!result) throw new ArgumentException("newUnitCodename already exists");

            var unit = new Unit
            {
                Codename = newUnitCodename
            };
            dataSet.Units.Add(unit);

            result = true;
        }
        catch (Exception ex)
        {
            result = false;
        }
        
        return result;
    }

    public static bool RemoveUnit(DataSet dataSet, string codeName, bool execute = false)
    {
        ArgumentNullException.ThrowIfNull(dataSet);
        ArgumentException.ThrowIfNullOrWhiteSpace(codeName);

        bool result;

        try
        {
            // We can only remove a new unit if it exists
            var unit = dataSet.Units.FirstOrDefault(x => x.Codename == codeName);
            result = unit != null;
            
            if (!execute) return result;
            if (!result) throw new ArgumentException("Unit not found");
            
            dataSet.Units.Remove(unit!);

            result = true;
        }
        catch (Exception ex)
        {
            result = false;
        }
        
        return result;
    }
}