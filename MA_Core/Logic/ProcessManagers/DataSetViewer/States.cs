using MA_Core.Data;

namespace MA_Core.Logic.ProcessManagers.DataSetViewer;

public abstract class DataSetViewerState : ICloneable
{
    public abstract object Clone();
}

public class EmptyState : DataSetViewerState
{
    public override object Clone() => new EmptyState();
}

public class LoadedState(DataSet dataSet, bool unload = false) : DataSetViewerState
{
    public DataSet DataSet { get; } = dataSet;
    public readonly bool Unload = unload;
    
    public override object Clone()
    {
        return new LoadedState(DataSet, Unload);
    }
}