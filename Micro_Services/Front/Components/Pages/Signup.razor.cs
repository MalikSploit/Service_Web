using Entities;
using Microsoft.AspNetCore.Components;
using Front.Services;

namespace Front.Components.Pages;

public partial class Signup : ComponentBase
{
    [Inject] private NavigationManager _NavigationManager { get; set; }
    
    private UserCreateModel _userCreateModel = new UserCreateModel();

    private async Task HandleSignup()
    {
        var apiUrl = LoginService.Urlprefix +  "api/User/register";

        HttpResponseMessage response = await Client.PostAsJsonAsync(apiUrl, _userCreateModel);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<User>();
            Console.WriteLine("Signup with success");
            _NavigationManager.NavigateTo("/login");
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Signup failed: {errorMessage}");
        }
    }
}