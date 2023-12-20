using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;

namespace Front.Components.Pages;

public partial class Index : ComponentBase
{
    [Inject] private ILocalStorageService LocalStorage { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    private bool _isDropdownVisible;
    private bool _isLoggedIn;

    protected override async Task OnInitializedAsync()
    {
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
        return !string.IsNullOrEmpty(jwtToken);
    }

    private async Task Logout()
    {
        await LocalStorage.RemoveItemAsync("jwtToken");
        NavigationManager.NavigateTo("/", true);
    }
}