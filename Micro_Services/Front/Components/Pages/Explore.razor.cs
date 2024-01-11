using Blazored.LocalStorage;
using Entities;
using Front.Services;
using Microsoft.AspNetCore.Components;

namespace Front.Components.Pages;

public partial class Explore : ComponentBase
{
    #pragma warning disable CS8618
    [Inject] private BookService BookService { get; set; }
    [Inject] private ILocalStorageService LocalStorage { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private LoginService LoginService { get; set; }
    [Inject] private CartStateService CartStateService { get; set; }

    private IEnumerable<Book> books;
    private IEnumerable<Book> filteredBooks;
    private bool _isLoggedIn;
    private string? searchTerm = string.Empty;
    private bool isDropdownOpen;
    private bool _isUserAdmin;
    private int CartItemCount { get; set; }

    protected override async Task OnInitializedAsync()
    {
        books = await BookService.GetBooksAsync();
        filteredBooks = books;
        _isUserAdmin = await LoginService.IsUserAdmin();
        _isLoggedIn = await LoginService.IsUserLoggedIn();

        CartStateService.OnChange += UpdateCartCount;
        UpdateCartCount();
    }

    private void UpdateSearch(ChangeEventArgs e)
    {
        searchTerm = e.Value?.ToString();
        filteredBooks = string.IsNullOrWhiteSpace(searchTerm) ? books : books.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
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

        
    private async Task Logout()
    {
        await LocalStorage.RemoveItemAsync("jwtToken");
        NavigationManager.NavigateTo("/", true);
    }
        
    private string GetDropdownClass()
    {
        return isDropdownOpen ? "block z-10" : "hidden";
    }

    private void ToggleDropdown()
    {
        isDropdownOpen = !isDropdownOpen;
    }
}