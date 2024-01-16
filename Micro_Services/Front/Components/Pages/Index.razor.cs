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
    private bool _isUserAdmin;
    private bool showAddToCartModal;
    private string modalMessage = "";
    private Timer modalTimer;
    private int CartItemCount { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var allBooks = await BookService.GetBooksAsync();
        Books = allBooks.Take(4);
        _isLoggedIn = await LoginService.IsUserLoggedIn();
        _isUserAdmin = await LoginService.IsUserAdmin();
        CartStateService.OnChange += UpdateCartCount;
        UpdateCartCount();
    }
    
    private async Task AddToCart(Book book)
    {
        var cart = await LocalStorage.GetItemAsync<Dictionary<int, int>>("cart") ?? new Dictionary<int, int>();

        if (!cart.TryAdd(book.Id, 1))
        {
            cart[book.Id] += 1;
        }

        await LocalStorage.SetItemAsync("cart", cart);
        await UpdateCartItemCount();

        if (_isLoggedIn)
        {
            // Synchronize the cart with the database
            await CartStateService.UpdateCartAsync(cart, updateServer: true);
        }

        // Show modal
        modalMessage = "Book added to cart!";
        showAddToCartModal = true;
        StateHasChanged();

        // Hide the modal after 1 second
        modalTimer?.Dispose();
        modalTimer = new Timer(_ => { 
            showAddToCartModal = false; 
            InvokeAsync(StateHasChanged); // Invoke StateHasChanged on the UI thread
        }, null, 1000, Timeout.Infinite);
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
        await CartStateService.ClearCartAsync();
        NavigationManager.NavigateTo("/", true);
    }
}