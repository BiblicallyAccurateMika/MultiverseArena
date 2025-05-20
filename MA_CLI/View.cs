using System.Text;
using MA_Core.Abstract;
using MA_Core.Util;

namespace Ma_CLI;

public abstract class View(View? parent)
{
    public record InteractionResult
    {
        public View? NewView { get; private init; }
        public InteractionResponse? InteractionResponse { get; private init; }
        
        private InteractionResult() { }
        public static InteractionResult Empty() => new();
        public static InteractionResult View(View view) => new() {NewView = view};
        public static InteractionResult Response(InteractionResponse response) => new() {InteractionResponse = response};
    }
    
    protected static InteractionResult done() => InteractionResult.Empty();
    protected static InteractionResult action(Action action) { action(); return done(); }
    protected static InteractionResult view(View view) => InteractionResult.View(view);
    protected static InteractionResult response(InteractionResponse response) => InteractionResult.Response(response);

    protected record Interaction(string Key, string Name, Func<string, InteractionResult> Action, Func<string?, bool>? KeyCheck = null);
        
    protected abstract string ViewName { get; }
    protected virtual Interaction[]? Interactions => null;

    protected abstract void render(StringBuilder builder);
    protected virtual InteractionResult exit() => InteractionResult.View(Parent!);

    protected virtual string Breadcrumbs => Parent == null ? ViewName : $"{Parent.Breadcrumbs} > {ViewName}";
    private View? Parent { get; } = parent;

    private IEnumerable<Interaction> AllInteractions
    {
        get
        {
            List<Interaction> interactions = [];
            if (Interactions != null) interactions.AddRange(Interactions);
            if (interactions.None(x => x.Key == "0")) interactions.Insert(0, new Interaction("0", "Exit", _ => exit()));
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

    public virtual void Process(InteractionResponse? response = null) { }
}

public abstract class View<TStateMachine, TState>(View? parent) : View(parent)
    where TStateMachine : StateMachine<TState>, new()
    where TState : StateHolder, new()
{
    protected TStateMachine StateMachine { get; } = new();

    public override void Process(InteractionResponse? response = null)
    {
        StateMachine.Run(response);
    }
}