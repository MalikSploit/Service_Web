using Front.Services;
using Microsoft.AspNetCore.Components;

namespace Front.Components.Pages;

public partial class Login : ComponentBase
{
    [Inject] private LoginService _loginService { get; set; }
    [Inject] private NavigationManager _NavigationManager { get; set; }

    private string email;
    private string pass;
    private string errorMessage;
    
    private async Task HandleLogin()
    {
        var userDto = await _loginService.AuthenticateUserAsync(email, pass);
        if (userDto != null)
        {
            Console.WriteLine("Signup with success");
            _NavigationManager.NavigateTo("/dashboard");
        }
        else
        {
            errorMessage = "Invalid username or password.";
        }
    }
}