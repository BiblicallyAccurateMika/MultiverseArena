using MA_Core.Util;

namespace MA_Core.Abstract;

public abstract record StateHolder;
public abstract record InteractionRequest;
public abstract record InteractionResponse;

public abstract class StateMachine<TStateHolder>(TStateHolder? initialState = null)
    where TStateHolder : StateHolder, new()
{
    public record TransitionResult(TStateHolder? NewState, InteractionRequest? Request = null);
    
    public delegate bool TransitionCondition(TStateHolder state);
    public delegate TransitionResult TransitionAction(TStateHolder state, InteractionResponse? response = null);
    public record Transition(TransitionCondition Condition, TransitionAction Action);
    
    protected Transition[] Transitions { get; init; } = [];
    private Transition? _transition;
    
    public TStateHolder StateHolder { get; private set; } = initialState ?? new TStateHolder();
    public InteractionRequest? Request { get; private set; }

    public void Run(InteractionResponse? response = null)
    {
        while (true)
        {
            if (_transition == null)
            {
                _transition = Transitions.FirstOrDefault(p => p.Condition(StateHolder));
                if (_transition == null) return; // No processes can be done, i.e. final state
            }

            var result = _transition.Action(StateHolder, response);
            if (result.NewState != null) StateHolder = result.NewState;
            Request = result.Request;

            if (result.Request != null) return; // Needs interaction
            _transition = null;
            
            response = null;
        }
    }
    
    protected static TransitionResult requestResult(InteractionRequest request) => new(null, request);
    protected static TransitionResult stateResult(TStateHolder state) => new(state);

    protected static TransitionBuilder<TStateHolder> buildTransition() => TransitionBuilder<TStateHolder>.Create();
}