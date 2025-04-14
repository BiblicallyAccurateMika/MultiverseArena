namespace MA_Core.Data;

public record Action
{
    public required string ID { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    
    public ActionStep[]? Steps { get; init; }
}