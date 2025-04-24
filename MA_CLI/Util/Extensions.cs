using Action = MA_Core.Data.Action;

namespace Ma_CLI.Util;

public static class Extensions
{
    public static string ToCliShortString(this Action action) => $"{action.ID} - {action.Name}";
}