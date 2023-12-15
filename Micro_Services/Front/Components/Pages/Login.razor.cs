using Front.Services;
using Microsoft.AspNetCore.Components;

namespace Front.Components.Pages;

public partial class Login : ComponentBase
{
    [Inject] private LoginService _loginService { get; set; }
    [Inject] private NavigationManager _NavigationManager { get; set; }

    private string email;
    private string password;
    private string errorMessage;
    
    private async Task HandleLogin()
    {
        var userDto = await _loginService.AuthenticateUserAsync(email, password);
        if (userDto != null)
        {
            // Ca marche :)
            _NavigationManager.NavigateTo("/dashboard");
        }
        else
        {
            // Erreur
            errorMessage = "Invalid username or password.";
        }
    }
}