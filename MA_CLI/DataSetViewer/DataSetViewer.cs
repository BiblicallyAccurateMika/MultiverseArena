using System.Text;
using Ma_CLI.Util;
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

    #region Views

    private abstract class DatasetView(DataSet data, View? parent) : View(parent)
    {
        protected DataSet Data { get; } = data;
    }

    private class OverviewView(DataSet data) : DatasetView(data, null)
    {
        protected override string ViewName => Data.Name;

        protected override Interaction[] Interactions =>
        [
            new("1", "View Actions", _ => new ActionsView(Data, this)),
            new("2", "View Units", _ => new UnitsView(Data, this))
        ];

        protected override void render(StringBuilder builder)
        {
            builder.AppendLine($"Path: {Data.Path}");
            builder.AppendLine($"Name: {Data.Name}");
            builder.AppendLine($"Actions: {Data.Actions.Count}");
            builder.AppendLine($"Units: {Data.Units.Count}");
        }
    }

    private class ActionsView(DataSet data, View parent) : DatasetView(data, parent)
    {
        protected override string ViewName => "ACTIONS";
        
        protected override Interaction[] Interactions => 
            [new("ID", "Action Details", id => new ActionDetailView(Data, this, id!), id => Data.Actions.Any(x => x.ID == id))];

        protected override void render(StringBuilder builder)
        {
            foreach (var action in Data.Actions)
            {
                builder.AppendLine(action.ToCliShortString());
            }
        }
    }

    private class ActionDetailView : DatasetView
    {
        protected override string ViewName => _action.ToCliShortString();
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

    private class UnitsView(DataSet data, View parent) : DatasetView(data, parent)
    {
        protected override string ViewName => "UNITS";

        protected override Interaction[] Interactions =>
        [
            new("CodeName", "Unit Details", codename => new UnitDetailView(Data, this, codename!), 
                codename => Data.Units.Any(x => x.Codename == codename)),
            new("Counter", "Unit Details (by counter)", counterStr =>
            {
                var counter = Int32.Parse(counterStr!) - 1;
                return new UnitDetailView(Data, this, Data.Units[counter].Codename);
            }, counterStr => Int32.TryParse(counterStr, out var counter) && counter > 0 && counter <= Data.Units.Count),
        ];
        
        protected override void render(StringBuilder builder)
        {
            for (var i = 0; i < Data.Units.Count; i++)
            {
                var unit = Data.Units[i];
                builder.AppendLine($"{i + 1} - {unit.Codename}");
            }
        }
    }
    
    private class UnitDetailView : DatasetView
    {
        protected override string ViewName => _unit.Codename;
        
        protected override Interaction[] Interactions => 
            [new("Action ID", "Action Details", id => new ActionDetailView(Data, this, id!), id => Data.Actions.Any(x => x.ID == id))];
        
        private readonly Unit _unit;
        
        public UnitDetailView(DataSet data, View parent, string codename) : base(data, parent)
        {
            _unit = Data.Units.First(x => x.Codename == codename);
        }
        
        protected override void render(StringBuilder builder)
        {
            builder.AppendLine($"Codename: {_unit.Codename}");
            builder.AppendLine($"Icon Path: {_unit.IconPath}");
            builder.AppendLine();
            builder.Append(renderLevelComparison());
        }

        private string renderLevelComparison()
        {
            var levels = new[] { _unit.Level1, _unit.Level2, _unit.Level3, _unit.Level4 };
            List<int> columnWidths = ["SpritePath".Length];
            columnWidths.AddRange(levels.Select(x => new[]
            {
                7, // Header width -> "Level x"
                x.Name.Length,
                x.SpritePath.Length,
                x.HP.ToString().Length,
                x.Strength.ToString().Length,
                x.Toughness.ToString().Length,
                x.Precision.ToString().Length,
                x.Agility.ToString().Length,
                x.Power.ToString().Length,
                x.Defense.ToString().Length,
                x.Aura.ToString().Length,
                x.Willpower.ToString().Length,
                x.Actions.Max(y => y.ToCliShortString().Length)
            }.Max()));

            var divider = "+" + String.Join("+", columnWidths.Select(width => new string('-', width + 2))) + "+";
            
            var sb = new StringBuilder();
            
            appendDivider();
            appendLine("", "Level 1", "Level 2", "Level 3", "Level 4");
            appendDivider();
            appendLine("Name", _unit.Level1.Name, _unit.Level2.Name, _unit.Level3.Name, _unit.Level4.Name);
            appendLine("SpritePath", _unit.Level1.SpritePath, _unit.Level2.SpritePath, _unit.Level3.SpritePath, _unit.Level4.SpritePath);
            appendDivider();
            appendLine("HP", _unit.Level1.HP.ToString(), _unit.Level2.HP.ToString(), _unit.Level3.HP.ToString(), _unit.Level4.HP.ToString());
            appendLine("Strength", _unit.Level1.Strength.ToString(), _unit.Level2.Strength.ToString(), _unit.Level3.Strength.ToString(), _unit.Level4.Strength.ToString());
            appendLine("Toughness", _unit.Level1.Toughness.ToString(), _unit.Level2.Toughness.ToString(), _unit.Level3.Toughness.ToString(), _unit.Level4.Toughness.ToString());
            appendLine("Precision", _unit.Level1.Precision.ToString(), _unit.Level2.Precision.ToString(), _unit.Level3.Precision.ToString(), _unit.Level4.Precision.ToString());
            appendLine("Agility", _unit.Level1.Agility.ToString(), _unit.Level2.Agility.ToString(), _unit.Level3.Agility.ToString(), _unit.Level4.Agility.ToString());
            appendLine("Power", _unit.Level1.Power.ToString(), _unit.Level2.Power.ToString(), _unit.Level3.Power.ToString(), _unit.Level4.Power.ToString());
            appendLine("Defense", _unit.Level1.Defense.ToString(), _unit.Level2.Defense.ToString(), _unit.Level3.Defense.ToString(), _unit.Level4.Defense.ToString());
            appendLine("Aura", _unit.Level1.Aura.ToString(), _unit.Level2.Aura.ToString(), _unit.Level3.Aura.ToString(), _unit.Level4.Aura.ToString());
            appendLine("Willpower", _unit.Level1.Willpower.ToString(), _unit.Level2.Willpower.ToString(), _unit.Level3.Willpower.ToString(), _unit.Level4.Willpower.ToString());
            appendDivider();
            var first = true;
            for (var i = 0; first || i < levels.Max(x => x.Actions.Length); i++)
            {
                var header = first ? "Actions" : "";
                if (first) first = false;
                
                var lvl1 = _unit.Level1.Actions.Length > i ? _unit.Level1.Actions[i].ToCliShortString() : "";
                var lvl2 = _unit.Level2.Actions.Length > i ? _unit.Level2.Actions[i].ToCliShortString() : "";
                var lvl3 = _unit.Level3.Actions.Length > i ? _unit.Level3.Actions[i].ToCliShortString() : "";
                var lvl4 = _unit.Level4.Actions.Length > i ? _unit.Level4.Actions[i].ToCliShortString() : "";
                
                appendLine(header, lvl1, lvl2, lvl3, lvl4);
            }
            appendDivider();
            
            return sb.ToString();
            
            void appendDivider() => sb.AppendLine(divider);
            void appendLine(string header, string lvl1, string lvl2, string lvl3, string lvl4)
            {
                var paddedHeader = header.PadRight(columnWidths[0]);
                var paddedLvl1 = lvl1.PadRight(columnWidths[1]);
                var paddedLvl2 = lvl2.PadRight(columnWidths[2]);
                var paddedLvl3 = lvl3.PadRight(columnWidths[3]);
                var paddedLvl4 = lvl4.PadRight(columnWidths[4]);
                sb.AppendLine($"| {paddedHeader} | {paddedLvl1} | {paddedLvl2} | {paddedLvl3} | {paddedLvl4} |");
            }
        }
    }

    #endregion
}