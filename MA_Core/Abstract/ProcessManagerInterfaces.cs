namespace MA_Core.Abstract;

public interface IInteractionRequest { }

public interface IInteractionResponse { }

public record HandleResult<TState>(
    TState NewState,
    bool IsComplete,
    IInteractionRequest? InteractionRequest = null
) where TState : class;

public interface IProcess<TState> where TState : class
{
    HandleResult<TState> Handle(TState currentState, IInteractionResponse? response = null);
    bool CanExecute(TState currentState);
}

public interface IProcessManager<TState> where TState : class
{
    TState CurrentState { get; }
    IInteractionRequest? CurrentRequest { get; }
    HandleResult<TState> Process(IInteractionResponse? response = null);
}