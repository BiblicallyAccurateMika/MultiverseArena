namespace MA_Core.Abstract;

public abstract record StateHolder;
public abstract record InteractionRequest;
public abstract record InteractionResponse;

public abstract class StateMachine<TStateHolder>(TStateHolder? initialState = null)
    where TStateHolder : StateHolder, new()
{
    protected record TransitionResult(TStateHolder NewState, InteractionRequest? Request = null);
    protected record Transition(Func<bool> Check, Func<InteractionResponse?, TransitionResult> Action);
    
    protected TransitionResult requestResult(InteractionRequest request) => new(StateHolder, request);
    protected TransitionResult stateResult(TStateHolder state) => new(state);
    protected TransitionResult currentStateResult() => new(StateHolder);
    
    protected abstract Transition[] Transitions { get; }
    private Transition? _transition;
    
    public TStateHolder StateHolder { get; private set; } = initialState ?? new TStateHolder();
    public InteractionRequest? Request { get; private set; }

    public void Run(InteractionResponse? response = null)
    {
        while (true)
        {
            if (_transition == null)
            {
                _transition = Transitions.FirstOrDefault(p => p.Check());
                if (_transition == null) return; // No processes can be done, i.e. final state
            }

            var result = _transition.Action(response);
            StateHolder = result.NewState;
            Request = result.Request;

            if (result.Request is not null) return; // Needs interaction
            _transition = null;
            
            response = null;
        }
    }
}