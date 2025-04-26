using MA_Core.Data;

namespace MA_Core.Logic.ProcessManagers.DataSetViewer;

public abstract class DataSetViewerState;

public class EmptyState : DataSetViewerState;

public class LoadedState(DataSet dataSet, bool unload = false) : DataSetViewerState
{
    public DataSet DataSet { get; } = dataSet;
    public readonly bool Unload = unload;
}