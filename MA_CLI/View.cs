using System.Text;
using MA_Core.Abstract;

namespace Ma_CLI;

public abstract class View(View? parent)
{
    public record InteractionResult
    {
        public View? NewView { get; private init; }
        public IInteractionResponse? InteractionResponse { get; private init; }
        
        private InteractionResult() { }
        public static InteractionResult Empty() => new();
        public static InteractionResult View(View view) => new() {NewView = view};
        public static InteractionResult Response(IInteractionResponse response) => new() {InteractionResponse = response};
    }
    
    protected static InteractionResult done() => InteractionResult.Empty();
    protected static InteractionResult action(Action action) { action(); return done(); }
    protected static InteractionResult view(View view) => InteractionResult.View(view);
    protected static InteractionResult response(IInteractionResponse response) => InteractionResult.Response(response);

    protected record Interaction(string Key, string Name, Func<string, InteractionResult> Action, Func<string?, bool>? KeyCheck = null);
        
    protected abstract string ViewName { get; }
    protected virtual Interaction[]? Interactions => null;

    protected abstract void render(StringBuilder builder);
    protected virtual InteractionResult exit() => InteractionResult.View(Parent!);

    protected virtual string Breadcrumbs => Parent == null ? ViewName : $"{Parent.Breadcrumbs} > {ViewName}";
    protected View? Parent { get; } = parent;

    private IEnumerable<Interaction> AllInteractions
    {
        get
        {
            List<Interaction> interactions = [new("0", "Exit", _ => exit())];
            if (Interactions != null) interactions.AddRange(Interactions);
            return interactions;
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

    public InteractionResult HandleInput(string? input)
    {
        foreach (var interaction in AllInteractions)
        {
            var check = interaction.KeyCheck ?? (key => key == interaction.Key);
            if (check(input)) return interaction.Action(input!);
        }

        throw new Exception("Invalid input!");
    }

    public virtual void Process(IInteractionResponse? response = null) { }
}

public abstract class View<TProcessManager, TState>(View? parent) : View(parent)
    where TProcessManager : IProcessManager<TState>, new()
    where TState : class
{
    protected TProcessManager ProcessManager { get; } = new();

    public override void Process(IInteractionResponse? response = null)
    {
        while (true)
        {
            var result = ProcessManager.Process(response);

            if (!result.IsComplete) return; // Exits if the ProcessManager needs user input

            response = null;
        }
    }
}