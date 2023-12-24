using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using Front.Services;
using Entities;

namespace Front.Components.Pages;

public partial class Index : ComponentBase
{
    #pragma warning disable CS8618
    [Inject] private ILocalStorageService LocalStorage { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private BookService BookService { get; set; }
    [Inject] private LoginService LoginService { get; set; }
    
    private IEnumerable<Book> Books;
    private bool _isDropdownVisible;
    private bool _isLoggedIn;
        

    protected override async Task OnInitializedAsync()
    {
        Books = await BookService.GetBooksAsync();
        _isLoggedIn = await LoginService.IsUserLoggedIn();
    }

    private void ToggleDropdown()
    {
        _isDropdownVisible = !_isDropdownVisible;
    }
    
    private string GetDropdownClass()
    {
        return _isDropdownVisible ? "block" : "hidden";
    }

    private async Task Logout()
    {
        await LocalStorage.RemoveItemAsync("jwtToken");
        NavigationManager.NavigateTo("/", true);
    }
}