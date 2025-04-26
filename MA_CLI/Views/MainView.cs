using System.Text;

namespace Ma_CLI.Views;

public class MainView() : View(null)
{
    protected override string ViewName => "Main";
    protected override Interaction[] Interactions => 
    [
        new("1", "DataSet Editor", _ => view(new DataSetView(this)))
    ];

    protected override InteractionResult exit()
    {
        Console.Clear();
        Environment.Exit(0);
        return InteractionResult.Empty();;
    }

    protected override void render(StringBuilder builder) { }
}