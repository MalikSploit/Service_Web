using System.Text;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using Front.Services;
using Entities;
using Newtonsoft.Json;

namespace Front.Components.Pages;

public partial class Index : ComponentBase
{
    #pragma warning disable CS8618
    [Inject] private ILocalStorageService LocalStorage { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private BookService BookService { get; set; }
    private IEnumerable<Book> Books;
    
    #pragma warning restore CS8618 
    private bool _isDropdownVisible;
    private bool _isLoggedIn;
        

    protected override async Task OnInitializedAsync()
    {
        Books = await BookService.GetBooksAsync();
        _isLoggedIn = await IsUserLoggedIn();
    }

    private void ToggleDropdown()
    {
        _isDropdownVisible = !_isDropdownVisible;
    }
    
    private string GetDropdownClass()
    {
        return _isDropdownVisible ? "block" : "hidden";
    }
    
    private async Task<bool> IsUserLoggedIn()
    {
        var jwtToken = await LocalStorage.GetItemAsStringAsync("jwtToken");
        if (string.IsNullOrEmpty(jwtToken))
        {
            return false;
        }

        try
        {
            var parts = jwtToken.Split('.');
            if (parts.Length == 3)
            {
                var payload = parts[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var jsonPayload = Encoding.UTF8.GetString(jsonBytes);
                var jwtPayload = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonPayload);

                if (jwtPayload != null && jwtPayload.TryGetValue("exp", out var expValue) &&
                    long.TryParse(expValue.ToString(), out var exp) &&
                    DateTimeOffset.FromUnixTimeSeconds(exp) > DateTimeOffset.UtcNow)
                {
                    return true;
                }
                
                await LocalStorage.RemoveItemAsync("jwtToken");
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error processing JWT token: " + ex.Message);
            return false;
        }
    }
    
    private byte[] ParseBase64WithoutPadding(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }

    private async Task Logout()
    {
        await LocalStorage.RemoveItemAsync("jwtToken");
        NavigationManager.NavigateTo("/", true);
    }
}