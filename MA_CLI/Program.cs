using Ma_CLI;
using Ma_CLI.Views;
using MA_Core.Abstract;

View view = new MainView();
InteractionResponse? response = null;
var errorMessage = String.Empty;

while (true)
{
    try
    {
        Console.Clear();
                
        view.Process(response);
        response = null;
        view.Render(errorMessage);
        errorMessage = "";
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
        errorMessage = e.Message;
        response = null;
    }
}