using System.Text;
using Ma_CLI.Views;
using MA_Core.Abstract;

namespace Ma_CLI;

public class Program
{
    public static void Main(string[] args)
    {
        var run = true;
        var endMessage = "";
        View view = new MainView();
        IInteractionResponse? response = null;

        while (run)
        {
            try
            {
                Console.Clear();
                
                view.Process(response);
                response = null;
                view.Render();
                var result = view.HandleInput(Console.ReadLine());

                if (result.NewView != null)
                {
                    view = result.NewView;
                }
                else if (result.InteractionResponse != null)
                {
                    response = result.InteractionResponse;
                }
            }
            catch (Exception e)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Error on main method!");
                sb.AppendLine(e.Message);
                endMessage = sb.ToString();
                run = false;
            }
        }
        
        Console.Clear();
        Console.WriteLine(endMessage);
    }
}
