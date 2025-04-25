using System.Text;
using MA_Core.Util;

namespace Ma_CLI;

public abstract class View(View? parent)
{
    protected record Interaction(string Key, string Name, Func<string?, View?> Action, Func<string?, bool>? KeyCheck = null);
        
    protected abstract string ViewName { get; }
    protected virtual Interaction[]? Interactions => null;

    protected abstract void render(StringBuilder builder);
        
    private string Breadcrumbs => Parent == null ? ViewName : $"{Parent.Breadcrumbs} > {ViewName}";
    private View? Parent { get; } = parent;

    private IEnumerable<Interaction> AllInteractions
    {
        get
        {
            var interactions = new List<Interaction>();
            if (Interactions != null) interactions.AddRange(Interactions);
            if (interactions.None(x => x.Key == "0")) interactions.Add(new Interaction("0", "Exit", _ => Parent));
            return interactions.OrderBy(x => x.Key);
        }
    }

    public void Render(string errorMessage = "")
    {
        var sb = new StringBuilder();
            
        sb.AppendLine($"==== {Breadcrumbs} ====");
        render(sb);
        sb.AppendLine();
        sb.AppendLine("---- Actions ----");
        foreach (var interaction in AllInteractions) sb.AppendLine($"[{interaction.Key}] {interaction.Name}");
        sb.AppendLine();
        if (errorMessage != "") sb.AppendLine(errorMessage);
        sb.Append("Action: ");
            
        Console.Write(sb);
    }

    public View? HandleInput(string? input)
    {
        foreach (var interaction in AllInteractions)
        {
            var check = interaction.KeyCheck ?? (key => key == interaction.Key);
            if (check(input)) return interaction.Action(input);
        }

        throw new Exception("Invalid input!");
    }
}