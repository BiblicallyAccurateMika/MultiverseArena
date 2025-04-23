using MA_Core.Abstract;
using MA_Core.Data;

namespace MA_Core.Logic.ProcessManagers.DataSetViewer.Processes;

public class LoadDataSet : IProcess<DataSetViewerState>
{
    public bool CanExecute(DataSetViewerState currentState)
    {
        switch (currentState)
        {
            case EmptyState:
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
            case SelectDataSetResponse selectResponse: return handleSelectResponse(currentState, selectResponse);
            default: throw new ArgumentException("Invalid response");
        }
    }
    
    private HandleResult<DataSetViewerState> handleInit(DataSetViewerState currentState)
    {
        var request = new SelectDataSetRequest();
        return new HandleResult<DataSetViewerState>(currentState, false, request);
    }
    
    private HandleResult<DataSetViewerState> handleSelectResponse(DataSetViewerState currentState, SelectDataSetResponse selectResponse)
    {
        try
        {
            return new HandleResult<DataSetViewerState>(new LoadedState(new DataSet(selectResponse.Path)), true);
        }
        catch (Exception e)
        {
            throw new FailedToLoadDataSetException(e.Message, e);
        }
    }
}