using MA_Core.Abstract;
using MA_Core.Logic.ProcessManagers.DataSetViewer.Processes;

namespace MA_Core.Logic.ProcessManagers.DataSetViewer;

public class DataSetViewerProcessManager : IProcessManager<DataSetViewerState>
{
    private readonly List<IProcess<DataSetViewerState>> _processes;
    private IProcess<DataSetViewerState>? _currentProcess = null;
    
    public DataSetViewerState CurrentState { get; private set; }
    public IInteractionRequest? CurrentRequest { get; private set; }

    public DataSetViewerProcessManager() : this(null) { }
    public DataSetViewerProcessManager(DataSetViewerState? initialState = null)
    {
        initialState ??= new EmptyState();
        
        CurrentState = initialState;
        _processes = [new LoadDataSet(), new UnloadDataSet(), new Idle()];
    }
    
    /// <summary>
    /// Possible Interactions:<br/>
    /// - <see cref="SelectDataSetRequest"/><br/>
    /// - <see cref="IdleRequest"/>
    /// </summary>
    public HandleResult<DataSetViewerState> Process(IInteractionResponse? response = null)
    {
        if (_currentProcess == null)
        {
            _currentProcess = _processes.FirstOrDefault(p => p.CanExecute(CurrentState));
            if (_currentProcess == null)
                return new HandleResult<DataSetViewerState>(CurrentState, true);
        }

        var result = _currentProcess.Handle(CurrentState, response);
        CurrentState = result.NewState;
        CurrentRequest = result.InteractionRequest;

        if (result.IsComplete)
        {
            _currentProcess = null;
        }
        
        return result;
    }
}