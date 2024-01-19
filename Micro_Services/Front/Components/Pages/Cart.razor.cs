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
    [Inject] private CartStateService CartStateService { get; set; }

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
        try
        {
            var storedCart = await LocalStorage.GetItemAsync<Dictionary<int, int>>("cart");
            if (storedCart != null)
            {
                var validCartItems = storedCart
                    .Where(kv => kv is { Key: >= 0, Value: >= 0 })
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                if (validCartItems.Count != storedCart.Count)
                {
                    await LocalStorage.SetItemAsync("cart", validCartItems);
                }

                cartItemIds = validCartItems;
            }
            else
            {
                cartItemIds = new Dictionary<int, int>();
            }

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
        catch (System.Text.Json.JsonException ex)
        {
            Console.WriteLine("Error reading cart from local storage: " + ex.Message);
            // Reset the cart in local storage to an empty dictionary
            cartItemIds = new Dictionary<int, int>();
            await LocalStorage.SetItemAsync("cart", cartItemIds);
            cartItems.Clear();
        }
    }


    private async Task UpdateQuantity(Book item, int quantity)
    {
        if (quantity < 0)
        {
            // Ignore the update
            return;
        }

        if (cartItemIds.ContainsKey(item.Id))
        {
            if (quantity == 0)
            {
                await RemoveItem(item);
            }
            else
            {
                cartItemIds[item.Id] = quantity;
                await LocalStorage.SetItemAsync("cart", cartItemIds);
                await LoadCartItems();

                if (_isLoggedIn)
                {
                    // Synchronize the updated cart with the server
                    await SynchronizeCartWithDatabase();
                }
            }
        }
    }
    
    private async Task SynchronizeCartWithDatabase()
    {
        try
        {
            await CartStateService.UpdateCartAsync(cartItemIds, updateServer: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error synchronizing cart with the server: " + ex.Message);
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

            if (_isLoggedIn)
            {
                // Synchronize the updated cart with the server
                await SynchronizeCartWithDatabase();
            }
        }
    }

    private bool CanCheckout => TotalPrice > 0;
    private Task Checkout()
    {
        if (CanCheckout)
        {
            NavigationManager.NavigateTo(_isLoggedIn ? "/Checkout" : "/Login");
            return Task.CompletedTask;
        }
        NavigationManager.NavigateTo("/Explore");
        return Task.CompletedTask;
    }

    private async Task Logout()
    {
        await LocalStorage.RemoveItemAsync("jwtToken");
        await CartStateService.ClearCartAsync();
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