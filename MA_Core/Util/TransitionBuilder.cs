using MA_Core.Abstract;

namespace MA_Core.Util;

public class TransitionBuilder<TStateHolder> where TStateHolder : StateHolder, new()
{
    #region Initialization

    private TransitionBuilder() { }

    public static TransitionBuilder<TStateHolder> Create() => new();

    public StateMachine<TStateHolder>.Transition Build()
    {
        return new StateMachine<TStateHolder>.Transition(state => _conditions.All(x => x(state)), (state, response) =>
        {
            if (response == null)
            {
                if (_initialAction != null)
                {
                    return _initialAction(state);
                }
            }
            else
            {
                var type = response.GetType();
                if (_actions.TryGetValue(type, out var action))
                {
                    return action(state, response);
                }
            }
            throw new ArgumentException("Invalid response!", nameof(response));
        });
    }

    #endregion

    #region Properties

    private readonly List<StateMachine<TStateHolder>.TransitionCondition> _conditions = [];
    private readonly Dictionary<Type, StateMachine<TStateHolder>.TransitionAction> _actions = [];
    private StateMachine<TStateHolder>.TransitionAction? _initialAction;

    #endregion
    
    #region Conditions

    public TransitionBuilder<TStateHolder> Condition(StateMachine<TStateHolder>.TransitionCondition condition)
    {
        _conditions.Add(condition);
        return this;
    }
    
    #endregion
    
    #region Actions

    public TransitionBuilder<TStateHolder> InitialAction(StateMachine<TStateHolder>.TransitionAction initialAction)
    {
        _initialAction = initialAction;
        return this;
    }

    public TransitionBuilder<TStateHolder> Action<TResponse>(Func<TStateHolder, TResponse, StateMachine<TStateHolder>.TransitionResult> action)
        where TResponse : InteractionResponse
    {
        _actions[typeof(TResponse)] = (state, response) => action(state, (response as TResponse)!);
        return this;
    }
    
    #endregion
}
