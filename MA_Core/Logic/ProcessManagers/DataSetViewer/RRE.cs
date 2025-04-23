using MA_Core.Abstract;

namespace MA_Core.Logic.ProcessManagers.DataSetViewer;

#region RRE for EmptyState

public record SelectDataSetRequest() : IInteractionRequest;
public record SelectDataSetResponse(string Path) : IInteractionResponse;

public class FailedToLoadDataSetException(string? message = null, Exception? innerException = null) : Exception(message, innerException);

#endregion

#region RRE for Idle

public record IdleRequest() : IInteractionRequest;
public record IdleResponse(bool UnloadDataSet = false) : IInteractionResponse;

#endregion