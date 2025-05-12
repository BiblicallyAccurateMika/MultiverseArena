using MA_Core.Abstract;
using MA_Core.Data;
using MA_Core.Logic.Managers;

namespace MA_Core.Logic.StateMachines;

#region Stateholder

public record DataSetEditorStateHolder : StateHolder
{
    public DataSetEditorStateHolder() : this(CurrentState: null) { }
    private DataSetEditorStateHolder(BaseState? CurrentState = null)
    {
        CurrentState ??= new EmptyState();
        this.CurrentState = CurrentState;
    }

    public abstract record BaseState;
    
    public record EmptyState : BaseState;
    public static DataSetEditorStateHolder Empty() => new(new EmptyState()); 
    
    public record LoadedState(DataSet DataSet) : BaseState;
    public static DataSetEditorStateHolder Loaded(DataSet dataSet) => new(new LoadedState(dataSet));
    
    public record UnloadState(DataSet DataSet) : BaseState;
    public static DataSetEditorStateHolder Unload(DataSet dataSet) => new(new UnloadState(dataSet));
    
    public BaseState CurrentState { get; }
}

#endregion

#region Requests, Responses, Exceptions

public record SelectDataSetRequest : InteractionRequest;
public record SelectDataSetResponse(string Path) : InteractionResponse;

public record IdleRequest : InteractionRequest;
public record IdleResponseUnload : InteractionResponse;
public record IdleResponseEdit(string Key, params string[] Args) : InteractionResponse;
public record IdleResponseSave : InteractionResponse;

#endregion

#region StateMachine

public class DataSetEditorStateMachine : StateMachine<DataSetEditorStateHolder>
{
    /// Base method, tests whether the object has the given type and returns the cast object
    private bool isState<TState>(out TState state)
    {
        if (StateHolder.CurrentState is TState baseState)
        {
            state = baseState;
            return true;
        }

        state = default!;
        return false;
    }
    /// Only the check, without the cast
    private bool isState<TState>() => isState<TState>(out _);
    /// Runs the type check and an additional check on the object
    private Func<bool> isState<TState>(Func<TState, bool> additionalCheck) => () => isState<TState>(out var state) && additionalCheck(state);

    protected override Transition[] Transitions =>
    [
        // Load DataSet
        new (isState<DataSetEditorStateHolder.EmptyState>, response =>
        {
            return response switch
            {
                null => requestResult(new SelectDataSetRequest()),
                SelectDataSetResponse selectResponse => stateResult(DataSetEditorStateHolder.Loaded(new DataSet(selectResponse.Path))),
                _ => throw new ArgumentException("Invalid response!", nameof(response))
            };
        }),
        // Idle
        new(isState<DataSetEditorStateHolder.LoadedState>, response =>
        {
            var state = (StateHolder.CurrentState as DataSetEditorStateHolder.LoadedState)!;
            switch (response)
            {
                case null:
                    return requestResult(new IdleRequest());
                case IdleResponseUnload:
                    return stateResult(DataSetEditorStateHolder.Unload(state.DataSet));
                case IdleResponseEdit edit:
                    DataSetManager.ExecuteEdit(state.DataSet, edit.Key, edit.Args);
                    return currentStateResult();
                case IdleResponseSave:
                    try
                    {
                        state.DataSet.Save();
                    }
                    catch (ArgumentException e) when (e.ParamName == nameof(state.DataSet.Path))
                    {
                        throw new Exception("Please provide a valid path.", e);
                    }
                    return currentStateResult();
                default:
                    throw new ArgumentException("Invalid response!", nameof(response));
            }
        }),
        // Unload DataSet
        new(isState<DataSetEditorStateHolder.UnloadState>, response =>
        {
            return response switch
            {
                null => stateResult(DataSetEditorStateHolder.Empty()),
                _ => throw new ArgumentException("Invalid response!", nameof(response))
            };
        })
    ];
}

#endregion