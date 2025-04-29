using MA_Core.Abstract;
using MA_Core.Data;

namespace MA_Core.Logic.ProcessManagers;

#region Stateholder

public record DataSetViewStateHolder : StateHolder
{
    public DataSetViewStateHolder() : this(CurrentState: null) { }
    public DataSetViewStateHolder(BaseState? CurrentState = null)
    {
        CurrentState ??= new EmptyState();
        this.CurrentState = CurrentState;
    }

    public abstract record BaseState;
    
    public record EmptyState : BaseState;
    public static DataSetViewStateHolder Empty() => new(new EmptyState()); 
    
    // Unload is only needed, because if we didn't have it we would automatically go into the Unload Process
    // todo: if there is more than one eligible process, return a request that ask which process should be run (bonus points if this behaviour can be configured)
    public record LoadedState(DataSet DataSet, bool Unload = false) : BaseState;
    public static DataSetViewStateHolder Loaded(DataSet dataSet, bool unload = false) => new(new LoadedState(dataSet, unload));

    public BaseState CurrentState { get; init; }
}

#endregion

#region Requests, Responses, Exceptions

public record SelectDataSetRequest : InteractionRequest;
public record SelectDataSetResponse(string Path) : InteractionResponse;

public record IdleRequest : InteractionRequest;
public record IdleResponse(bool UnloadDataSet = false) : InteractionResponse;

#endregion

#region ProcessManager

public class DataSetViewProcessManager : ProcessManager<DataSetViewStateHolder>
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

    protected override Process[] Processes =>
    [
        // Load DataSet
        new (isState<DataSetViewStateHolder.EmptyState>, response =>
        {
            return response switch
            {
                null => request(new SelectDataSetRequest()),
                SelectDataSetResponse selectResponse => state(DataSetViewStateHolder.Loaded(new DataSet(selectResponse.Path))),
                _ => throw new ArgumentException("Invalid response!", nameof(response))
            };
        }),
        // Unload DataSet
        new(isState<DataSetViewStateHolder.LoadedState>(state => state.Unload), response =>
        {
            return response switch
            {
                null => state(DataSetViewStateHolder.Empty()),
                _ => throw new ArgumentException("Invalid response!", nameof(response))
            };
        }),
        // Idle
        new(isState<DataSetViewStateHolder.LoadedState>, response =>
        {
            return response switch
            {
                null => request(new IdleRequest()),
                IdleResponse idleResponse =>
                    state(DataSetViewStateHolder.Loaded((StateHolder.CurrentState as DataSetViewStateHolder.LoadedState)!.DataSet, idleResponse.UnloadDataSet)),
                _ => throw new ArgumentException("Invalid response!", nameof(response))
            };
        })
    ];
}

#endregion