using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using Front.Services;
using Entities;

namespace Front.Components.Pages;

public partial class Index : ComponentBase
{
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
    [Inject] private ILocalStorageService LocalStorage { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private BookService BookService { get; set; }
    private IEnumerable<Book> Books;
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

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
        return !string.IsNullOrEmpty(jwtToken);
    }

    private async Task Logout()
    {
        await LocalStorage.RemoveItemAsync("jwtToken");
        NavigationManager.NavigateTo("/", true);
    }
}