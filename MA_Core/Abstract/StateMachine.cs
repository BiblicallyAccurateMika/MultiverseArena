using MA_Core.Util;

namespace MA_Core.Abstract;

public abstract record StateHolder;
public abstract record InteractionRequest;
public abstract record InteractionResponse;

public abstract class StateMachine<TStateHolder>(TStateHolder? initialState = null)
    where TStateHolder : StateHolder, new()
{
    public abstract record TransitionResult;
    public record TransitionResultRequest(InteractionRequest InteractionRequest) : TransitionResult;
    public record TransitionResultState(TStateHolder StateHolder) : TransitionResult;
    
    public delegate bool TransitionCondition(TStateHolder state);
    public delegate TransitionResult TransitionAction(TStateHolder state, InteractionResponse? response = null);
    public record Transition(string Name, TransitionCondition Condition, TransitionAction Action);
    
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

            Request = null;
            switch (result)
            {
                case TransitionResultRequest resultRequest:
                    Request = resultRequest.InteractionRequest;
                    break;
                case TransitionResultState resultState:
                    StateHolder = resultState.StateHolder;
                    break;
            }

            if (Request != null) return; // Needs interaction
            _transition = null;
            
            response = null;
        }
    }
    
    protected static TransitionResultRequest requestResult(InteractionRequest request) => new(request);
    protected static TransitionResultState stateResult(TStateHolder state) => new(state);

    protected static TransitionBuilder<TStateHolder> buildTransition(string name) => TransitionBuilder<TStateHolder>.Create(name);
}