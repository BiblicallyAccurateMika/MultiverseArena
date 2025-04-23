using System.Text;
using MA_Core.Abstract;
using MA_Core.Data;
using MA_Core.Logic.ProcessManagers.DataSetViewer;
using MA_Core.Util;
using Action = MA_Core.Data.Action;

// ReSharper disable once CheckNamespace
namespace Ma_CLI;

public static class DataSetViewer
{
    private class ExitException : Exception;
    
    public static void Run()
    {
        DataSetViewerProcessManager processManager = new();
        IInteractionResponse? response = null;

        while (true)
        {
            try
            {
                var result = processManager.Process(response);

                if (result.IsComplete)
                {
                    response = null;
                }
                else
                {
                    switch (result.InteractionRequest)
                    {
                        case SelectDataSetRequest:
                            response = handleSelect();
                            break;
                        case IdleRequest:
                            response = handleIdle((LoadedState) processManager.CurrentState);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (ExitException)
            {
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
                response = null;
                Thread.Sleep(1000);
            }
        }
    }

    private static SelectDataSetResponse handleSelect()
    {
        Console.Clear();
        Console.WriteLine("Select DataSet");
        Console.Write("Path: ");

        var input = readInput();
        if (input == "0") throw new ExitException();
        return new SelectDataSetResponse(input);
    }

    private static IdleResponse handleIdle(LoadedState state)
    {
        View? view = new OverviewView(state.DataSet);
        var errorMessage = "";

        while (true)
        {
            try
            {
                if (view is null)
                {
                    return new IdleResponse(true);
                }
                
                Console.Clear();
                view.Render(errorMessage);
                view = view.HandleInput(readInput());
                
                errorMessage = "";
            }
            catch (Exception e)
            {
                if (typeof(ExitException) == e.GetType()) throw;
                errorMessage = e.Message;
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }
    
    private static string readInput()
    {
        var input = Console.ReadLine();
        if (input == "exit") throw new ExitException();
        input ??= String.Empty;
        return input;
    }
    
    private abstract class View(DataSet data, View? parent)
    {
        protected record Interaction(string Key, string Name, Func<string?, View?> Action, Func<string?, bool>? KeyCheck = null);
        
        protected abstract string ViewName { get; }
        protected virtual Interaction[]? Interactions => null;

        protected abstract void render(StringBuilder builder);
        
        private string Breadcrumbs => Parent == null ? ViewName : $"{Parent.Breadcrumbs} > {ViewName}";
        protected DataSet Data { get; } = data;
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

    private class OverviewView(DataSet data) : View(data, null)
    {
        protected override string ViewName => "OVERVIEW";

        protected override Interaction[] Interactions =>
        [
            new("1", "View Actions", _ => new ActionsView(Data, this)),
            new("2", "View Units", _ => new UnitsView(Data, this))
        ];

        protected override void render(StringBuilder builder)
        {
            builder.AppendLine($"Path: {Data.Path}");
            builder.AppendLine($"Actions: {Data.Actions.Count}");
            builder.AppendLine($"Units: {Data.Units.Count}");
        }
    }

    private class ActionsView(DataSet data, View parent) : View(data, parent)
    {
        protected override string ViewName => "ACTIONS";
        
        protected override Interaction[] Interactions => 
            [new("ID", "Action Details", id => new ActionDetailView(Data, this, id!), id => Data.Actions.Any(x => x.ID == id))];

        protected override void render(StringBuilder builder)
        {
            foreach (var action in Data.Actions)
            {
                builder.AppendLine($"{action.ID} - {action.Name}");
            }
        }
    }

    private class ActionDetailView : View
    {
        protected override string ViewName => _action.Name;
        private readonly Action _action;
        
        public ActionDetailView(DataSet data, View parent, string id) : base(data, parent)
        {
            _action = Data.Actions.First(x => x.ID == id);
        }
        
        protected override void render(StringBuilder builder)
        {
            builder.AppendLine($"ID: {_action.ID}");
            builder.AppendLine($"Name: {_action.Name}");
            builder.AppendLine($"Description: {_action.Description}");
        }
    }

    private class UnitsView(DataSet data, View parent) : View(data, parent)
    {
        protected override string ViewName => "UNITS";

        protected override Interaction[] Interactions =>
            [new("CodeName", "Unit Details", codename => new UnitDetailView(Data, this, codename!),
                codename => Data.Units.Any(x => x.Codename == codename))];
        
        protected override void render(StringBuilder builder)
        {
            foreach (var unit in Data.Units)
            {
                builder.AppendLine($"{unit.Codename}");
            }
        }
    }
    
    private class UnitDetailView : View
    {
        protected override string ViewName => _unit.Codename;
        private readonly Unit _unit;
        
        public UnitDetailView(DataSet data, View parent, string codename) : base(data, parent)
        {
            _unit = Data.Units.First(x => x.Codename == codename);
        }
        
        protected override void render(StringBuilder builder)
        {
            builder.AppendLine($"Codename: {_unit.Codename}");
            builder.AppendLine($"Icon Path: {_unit.IconPath}");
            
            //todo: Show level detail
        }
    }
}