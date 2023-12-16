using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace Front.Components.Pages;

public partial class Logged
{
    private string _userName = "";

    [Inject]
    NavigationManager? NavigationManager { set; get; }
    [Inject]
    private ILocalStorageService? LocalStorage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var jwtToken = await LocalStorage.GetItemAsStringAsync("jwtToken");
        Console.WriteLine($"Retrieved JWT token: {jwtToken}");
        if (!string.IsNullOrEmpty(jwtToken) && IsTokenValid(jwtToken))
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(jwtToken);
                _userName = $"{jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value} {jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "surname")?.Value}";
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error parsing JWT token: " + ex.Message);
            }
        }
        else
        {
            NavigationManager.NavigateTo("/Login");
        }
    }

    private bool IsTokenValid(string token)
    {
        // Implement token validation logic here
        return !string.IsNullOrEmpty(token);
    }
}