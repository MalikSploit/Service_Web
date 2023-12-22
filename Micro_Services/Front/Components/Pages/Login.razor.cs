using Front.Services;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;

namespace Front.Components.Pages;

public partial class Login : ComponentBase
{
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
    [Inject] private LoginService LoginService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private ILocalStorageService LocalStorage { get; set; }
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

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
        
        
        if (isSuccess && userDto?.Token != null)
        {
            // Store the token in local storage
            await LocalStorage.SetItemAsync("jwtToken", userDto.Token);
            
            NavigationManager.NavigateTo("/");
        }
        else
        {
            _attemptCount++;
            _errorMessage = error;
        }
    }
}