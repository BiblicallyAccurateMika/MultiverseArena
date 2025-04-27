using System.Text;
using Ma_CLI.Util;
using MA_Core.Data;
using MA_Core.Logic.ProcessManagers.DataSetViewer;
using Action = MA_Core.Data.Action;

namespace Ma_CLI.Views;

public class DataSetView(View parent) : View<DataSetViewerProcessManager, DataSetViewerState>(parent)
{
    protected override string ViewName => "DataSet Viewer";
    protected override Interaction[] Interactions
    {
        get
        {
            switch (ProcessManager.CurrentRequest)
            {
                case SelectDataSetRequest:
                    return
                    [
                        new Interaction("ID", "View Dataset",
                            idStr => response(new SelectDataSetResponse(DataSets[Int32.Parse(idStr) - 1])),
                            idStr => Int32.TryParse(idStr, out var id) && id > 0 && id <= DataSets.Length),
                        new Interaction("Path", "View Dataset (by path)",
                            path => response(new SelectDataSetResponse(path)),
                            path => !String.IsNullOrWhiteSpace(path))
                    ];
                case IdleRequest:
                    //todo: maybe add Dataset to Request so we dont have to cast it here
                    var data = (ProcessManager.CurrentState as LoadedState)!.DataSet;
                    return
                    [
                        new Interaction("1", "View Actions", _ => view(new ActionsView(this, data))),
                        new Interaction("2", "View Units", _ => view(new UnitsView(this, data)))
                    ];
                default: return null!;
            }
        }
    }

    protected override InteractionResult exit()
    {
        switch (ProcessManager.CurrentState)
        {
            case LoadedState: return response(new IdleResponse(true));
            default: return base.exit();
        }
    }

    protected override void render(StringBuilder builder)
    {
        switch (ProcessManager.CurrentState)
        {
            case EmptyState:
                for (var i = 0; i < DataSets.Length; i++)
                {
                    var dataSet = DataSets[i];

                    builder.AppendLine($"{i + 1} - {Path.GetFileName(dataSet)}");
                }
                break;
            case LoadedState loaded:
                builder.AppendLine($"Path: {loaded.DataSet.Path}");
                builder.AppendLine($"Name: {loaded.DataSet.Name}");
                builder.AppendLine($"Actions: {loaded.DataSet.Actions.Count}");
                builder.AppendLine($"Units: {loaded.DataSet.Units.Count}");
                break;
        }
    }

    private string[]? _dataSets;
    private string[] DataSets => _dataSets ??= DataSet.GetDataSetsFromFolder(Settings.Instance.DataSetFolderPath);

    #region Subviews

    private abstract class DatasetView(View parent, DataSet data) : View(parent)
    {
        protected DataSet Data { get; } = data;
    }

    private class ActionsView(View parent, DataSet data) : DatasetView(parent, data)
    {
        protected override string ViewName => "ACTIONS";
        
        protected override Interaction[] Interactions => 
            [new("ID", "Action Details", id => view(new ActionDetailView(this, Data, id)), id => Data.Actions.Any(x => x.ID == id))];

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
        
        public ActionDetailView(View parent, DataSet data, string id) : base(parent, data)
        {
            _action = Data.Actions.First(x => x.ID == id);
        }
        
        protected override void render(StringBuilder builder)
        {
            builder.AppendLine($"ID: {_action.ID}");
            builder.AppendLine($"Name: {_action.Name}");
            builder.AppendLine($"Description: {_action.Description}");
            
            //todo: show the actionplan
        }
    }

    private class UnitsView(View parent, DataSet data) : DatasetView(parent, data)
    {
        protected override string ViewName => "UNITS";

        protected override Interaction[] Interactions =>
        [
            new("CodeName", "Unit Details", codename => view(new UnitDetailView(this, Data, codename)), 
                codename => Data.Units.Any(x => x.Codename == codename)),
            new("Counter", "Unit Details (by counter)", counterStr =>
            {
                var counter = Int32.Parse(counterStr) - 1;
                return view(new UnitDetailView(this, Data, Data.Units[counter].Codename));
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
            [new("Action ID", "Action Details", id => view(new ActionDetailView(this, Data, id)), id => Data.Actions.Any(x => x.ID == id))];
        
        private readonly Unit _unit;
        
        public UnitDetailView(View parent, DataSet data, string codename) : base(parent, data)
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