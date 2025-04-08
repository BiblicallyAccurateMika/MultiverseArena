namespace MA_Core.Data;

public class Action
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public ActionStep[] Steps { get; set; }
}