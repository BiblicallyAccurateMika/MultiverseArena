using MA_Core.Abstract;

namespace MA_Core.Logic.ProcessManagers.DataSetViewer.Processes;

public class UnloadDataSet : IProcess<DataSetViewerState>
{
    public bool CanExecute(DataSetViewerState currentState)
    {
        switch (currentState)
        {
            case LoadedState state:
                return state.Unload;
            default:
                return false;
        }
    }
    
    public HandleResult<DataSetViewerState> Handle(DataSetViewerState currentState, IInteractionResponse? response)
    {
        ArgumentNullException.ThrowIfNull(currentState);

        switch (response)
        {
            case null: return handleUnload(currentState);
            default: throw new ArgumentException("Invalid response");
        }
    }
    
    private HandleResult<DataSetViewerState> handleUnload(DataSetViewerState currentState)
    {
        return new HandleResult<DataSetViewerState>(new EmptyState(), true);
    }
}