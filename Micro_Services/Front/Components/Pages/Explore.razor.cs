using Blazored.LocalStorage;
using Entities;
using Front.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;

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
    private bool showAddToCartModal;
    private string modalMessage = "";
    private Timer modalTimer;

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
        searchTerm = e.Value?.ToString()!;

        if(searchTerm.IsNullOrEmpty() || searchTerm.Length <= 3 || !books.Any()) { filteredBooks = books; return; }

        var keywords = searchTerm.ToLowerInvariant().Split(" ");

        List<Tuple<Book, int>> BookWithScore = new(books.Count());
        BookWithScore.AddRange(from b in books let titles = b.Title.ToLowerInvariant().Split(" ") let description = b.Description.ToLowerInvariant().Split(" ") let author = b.Author.ToLowerInvariant().Split(" ") let book_keywords = titles.Union(description).Union(author) let score = keywords.Sum(kw => book_keywords.Select(bwk => StringDistance.LevenshteinDistance(kw, bwk)).Prepend(int.MaxValue).Min()) select new Tuple<Book, int>(b, score));

        BookWithScore.Sort((a,b) => a.Item2.CompareTo(b.Item2));

        filteredBooks = BookWithScore.Select(t => t.Item1);

        //filteredBooks = string.IsNullOrWhiteSpace(searchTerm) ? books : books.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
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
        modalTimer.Dispose();
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

        try
        {
            CartItemCount = cart.Values.Sum();
            StateHasChanged(); // Notify the component to refresh the UI
        }
        catch(Exception ex) // In case of overflow
        {
            Console.WriteLine($"Error : {ex}");
        }
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