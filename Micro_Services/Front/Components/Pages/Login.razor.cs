using Front.Services;
using Microsoft.AspNetCore.Components;

namespace Front.Components.Pages;

public partial class Login : ComponentBase
{
    [Inject] private LoginService? LoginService { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }

    private string _email="";
    private string _pass="";
    private string _errorMessage="";
    private int _attemptCount;
    
    private async Task HandleLoginAsync()
    {
        if (_attemptCount >= 5)
        {
            _errorMessage = "You've reached the maximum number of login attempts. Please try again later.";
            return;
        }

        var (isSuccess, userDto, error) = await LoginService.AuthenticateUserAsync(_email, _pass);
        if (isSuccess)
        {
            if (NavigationManager is not null)
            {
                NavigationManager.NavigateTo("/Logged");
            }
            else
            {
                Console.WriteLine("Navigation Manager is Null");
            }
        }
        else
        {
            _attemptCount++;
            _errorMessage = error;
        }
    }
}