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
    [Inject] private CartStateService CartStateService { get; set; }
    
    private IEnumerable<Book> Books;
    private bool _isDropdownVisible;
    private bool _isLoggedIn;
    private int CartItemCount { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var allBooks = await BookService.GetBooksAsync();
        Books = allBooks.Take(4);
        _isLoggedIn = await LoginService.IsUserLoggedIn();
        CartStateService.OnChange += UpdateCartCount;
        UpdateCartCount();
    }
    
    private async Task AddToCart(Book book)
    {
        var cart = await LocalStorage.GetItemAsync<Dictionary<int, int>>("cart") ?? new Dictionary<int, int>();

        // Add or update the quantity for the given book
        if (!cart.TryAdd(book.Id, 1))
        {
            cart[book.Id] += 1;
        }

        // If it's not, add it with a quantity of 1
        await LocalStorage.SetItemAsync("cart", cart);
        await UpdateCartItemCount();
    }
    
    private async void UpdateCartCount()
    {
        CartItemCount = await CartStateService.GetCartItemCountAsync();
        StateHasChanged();
    }
    
    private async Task UpdateCartItemCount()
    {
        var cart = await LocalStorage.GetItemAsync<Dictionary<int, int>>("cart") ?? new Dictionary<int, int>();
        CartItemCount = cart.Values.Sum();
        StateHasChanged(); // Notify the component to refresh the UI
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