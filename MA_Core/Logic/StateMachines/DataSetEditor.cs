using MA_Core.Abstract;
using MA_Core.Data;
using MA_Core.Logic.Managers;
using MA_Core.Util;

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
    public LoadedState AsLoaded() => (CurrentState as LoadedState)!;
    
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
    public DataSetEditorStateMachine()
    {
        Transitions = [loadDataSet(), idle(), unload()];
    }

    private static Transition loadDataSet()
    {
        return buildTransition("LoadDataSet")
            .Condition(state => state.CurrentState is DataSetEditorStateHolder.EmptyState)
            .InitialAction((_, _) => requestResult(new SelectDataSetRequest()))
            .Action<SelectDataSetResponse>((_, selectResponse) => stateResult(DataSetEditorStateHolder.Loaded(new DataSet(selectResponse.Path))))
            .Build();
    }

    private static Transition idle()
    {
        return buildTransition("Idle")
            .Condition(state => state.CurrentState is DataSetEditorStateHolder.LoadedState)
            .InitialAction((_, _) => requestResult(new IdleRequest()))
            .Action<IdleResponseUnload>((state, _) => stateResult(DataSetEditorStateHolder.Unload(state.AsLoaded().DataSet)))
            .Action<IdleResponseEdit>((state, edit) =>
            {
                DataSetManager.ExecuteEdit(state.AsLoaded().DataSet, edit.Key, edit.Args);
                return stateResult(state);
            })
            .Action<IdleResponseSave>((state, _) =>
            {
                try
                {
                    state.AsLoaded().DataSet.Save();
                }
                catch (ArgumentException e) when (e.ParamName == nameof(DataSetEditorStateHolder.LoadedState.DataSet.Path))
                {
                    throw new Exception("Please provide a valid path.", e);
                }
                return stateResult(state);
            })
            .Build();
    }

    private static Transition unload()
    {
        return buildTransition("Unload")
            .Condition(state => state.CurrentState is DataSetEditorStateHolder.UnloadState)
            .InitialAction((_, _) => stateResult(DataSetEditorStateHolder.Empty()))
            .Build();
    }
}

#endregion