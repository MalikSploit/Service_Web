using Blazored.LocalStorage;
using Entities;
using Front.Services;
using Microsoft.AspNetCore.Components;

namespace Front.Components.Pages;

public partial class Cart : ComponentBase
{
    #pragma warning disable CS8618
    [Inject] private ILocalStorageService LocalStorage { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private BookService BookService { get; set; }
    [Inject] private LoginService LoginService { get; set; }

    private Dictionary<int, int> cartItemIds = new();
    private readonly List<Book> cartItems = []; 
    private bool _isLoggedIn;
    private bool _isUserAdmin;
    private bool isDropdownOpen;

    private decimal TotalPrice
    {
        get
        {
            return cartItems
                .Where(item => cartItemIds.ContainsKey(item.Id))
                .Sum(item => item.Price * cartItemIds[item.Id]);
        }
    }
        
    protected override async Task OnInitializedAsync()
    {
        _isLoggedIn = await LoginService.IsUserLoggedIn();
        _isUserAdmin = await LoginService.IsUserAdmin();
        await LoadCartItems();
    }

    private async Task LoadCartItems()
    {
        var storedCart = await LocalStorage.GetItemAsync<Dictionary<int, int>>("cart");
        cartItemIds = storedCart ?? new Dictionary<int, int>();
    
        cartItems.Clear();
        foreach (var itemId in cartItemIds.Keys)
        {
            var book = await BookService.GetBookByIdAsync(itemId);
            if (book != null)
            {
                cartItems.Add(book);
            }
        }
    }

    private async Task UpdateQuantity(Book item, int quantity)
    {
        if (cartItemIds.ContainsKey(item.Id))
        {
            if (quantity <= 0)
            {
                await RemoveItem(item);
            }
            else
            {
                cartItemIds[item.Id] = quantity;
                await LocalStorage.SetItemAsync("cart", cartItemIds);
                await LoadCartItems();
            }
        }
    }

    private async Task RemoveItem(Book item)
    {
        if (cartItemIds.ContainsKey(item.Id))
        {
            cartItemIds.Remove(item.Id);
            cartItems.RemoveAll(b => b.Id == item.Id);
            await LocalStorage.SetItemAsync("cart", cartItemIds);
            StateHasChanged();
        }
    }

    private Task Checkout()
    {
        NavigationManager.NavigateTo("/Checkout");
        return Task.CompletedTask;
        // Checkout logic here
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