namespace MA_Core.Util;

public static class TempDir
{
    private const string TempPrefix = "multiverseArena_";
    private static readonly DirectoryInfo BaseDir = Directory.CreateTempSubdirectory(TempPrefix);

    public static DirectoryInfo GetNewTempDir(string prefix)
    {
        var name = $"{prefix}_{Guid.NewGuid():N}";
        var newTempDir = BaseDir.CreateSubdirectory(name);
        CleanTempDirs();
        return newTempDir;
    }

    /// <summary>
    /// Removes the oldest temp directories, so that only 5 are kept at any time
    /// </summary>
    public static void CleanTempDirs()
    {
        const int maxDirCount = 5;
        var tempDir = Path.GetTempPath();
        var dirPaths = Directory.GetDirectories(tempDir, $"{TempPrefix}*", SearchOption.TopDirectoryOnly);
        if (dirPaths.Length <= maxDirCount) return;
        var dirs = dirPaths.Select(dirPath => new DirectoryInfo(dirPath)).OrderByDescending(dir => dir.CreationTime);
        dirs.ToList()[(maxDirCount-1)..].ForEach(dir => dir.Delete(true));
    }
}