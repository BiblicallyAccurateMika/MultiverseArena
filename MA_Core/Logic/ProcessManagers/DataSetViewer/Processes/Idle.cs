using MA_Core.Abstract;

namespace MA_Core.Logic.ProcessManagers.DataSetViewer.Processes;

public class Idle : IProcess<DataSetViewerState>
{
    public bool CanExecute(DataSetViewerState currentState)
    {
        switch (currentState)
        {
            case LoadedState:
                return true;
            default:
                return false;
        }
    }
    
    public HandleResult<DataSetViewerState> Handle(DataSetViewerState currentState, IInteractionResponse? response)
    {
        ArgumentNullException.ThrowIfNull(currentState);

        switch (response)
        {
            case null: return handleInit(currentState);
            case IdleResponse idleResponse: return handleResponse(currentState, idleResponse);
            default: throw new ArgumentException("Invalid response");
        }
    }
    
    private HandleResult<DataSetViewerState> handleInit(DataSetViewerState currentState)
    {
        var request = new IdleRequest();
        return new HandleResult<DataSetViewerState>(currentState, false, request);
    }
    
    private HandleResult<DataSetViewerState> handleResponse(DataSetViewerState currentState, IdleResponse idleResponse)
    {
        var newState = new LoadedState(((LoadedState)currentState).DataSet, unload:true);
        return new HandleResult<DataSetViewerState>(newState, true);
    }
}