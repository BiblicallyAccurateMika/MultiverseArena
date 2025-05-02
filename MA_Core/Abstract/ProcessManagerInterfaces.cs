namespace MA_Core.Abstract;

public abstract record StateHolder;
public abstract record InteractionRequest;
public abstract record InteractionResponse;

public abstract class ProcessManager<TStateHolder>(TStateHolder? initialState = null)
    where TStateHolder : StateHolder, new()
{
    public record ProcessResult(TStateHolder NewState, InteractionRequest? Request = null);
    public record Process(Func<bool> Check, Func<InteractionResponse?, ProcessResult> Action);
    
    protected ProcessResult requestResult(InteractionRequest request) => new(StateHolder, request);
    protected ProcessResult stateResult(TStateHolder state) => new(state);
    protected ProcessResult currentStateResult() => new(StateHolder);
    
    protected abstract Process[] Processes { get; }
    private Process? _process;
    
    public TStateHolder StateHolder { get; private set; } = initialState ?? new TStateHolder();
    public InteractionRequest? Request { get; private set; }

    public void Run(InteractionResponse? response = null)
    {
        if (_process == null)
        {
            _process = Processes.FirstOrDefault(p => p.Check());
            if (_process == null) return; // No processes can be done, i.e. final state
        }
        
        var result = _process.Action(response);
        StateHolder = result.NewState;
        Request = result.Request;
            
        if (result.Request is not null) return; // Needs interaction
        _process = null;
        Run();
    }
}