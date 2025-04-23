namespace Ma_CLI;

public class Program
{
    private record ActionEntry(string Name, Action Action);
    
    public static void Main(string[] args)
    {
        var run = true;

        var actions = new List<ActionEntry>
        {
            new("Exit", () => run = false),
            new("Viewer", DataSetViewer.Run)
        };

        while (run)
        {
            try
            {
                Console.Clear();
                
                for (var i = 0; i < actions.Count; i++)
                {
                    Console.WriteLine($"[{i}] {actions[i].Name}");
                }

                var input = Console.ReadLine();

                if (Int32.TryParse(input, out var result) && result >= 0 && result < actions.Count)
                {
                    try
                    {
                        actions[result].Action();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in action!");
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error on main method!");
                Console.WriteLine(e);
                run = false;
            }
        }
        
        Console.Clear();
    }
}
