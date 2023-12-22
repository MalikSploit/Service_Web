using Blazored.LocalStorage;
using Front.Services;
using Microsoft.AspNetCore.Components;

namespace Front.Components.Pages
{
    public partial class Explore : ComponentBase
    {
        [Inject] private BookService _BookService { get; set; }
        [Inject] private ILocalStorageService LocalStorage { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        private bool _isLoggedIn;
        
        private IEnumerable<Book> books;
        private IEnumerable<Book> filteredBooks;
        private string searchTerm = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            books = await _BookService.GetBooksAsync();
            filteredBooks = books;
            _isLoggedIn = await IsUserLoggedIn();
        }

        private async Task<bool> IsUserLoggedIn()
        {
            var jwtToken = await LocalStorage.GetItemAsStringAsync("jwtToken");
            return !string.IsNullOrEmpty(jwtToken);
        }

        private void UpdateSearch(ChangeEventArgs e)
        {
            searchTerm = e.Value.ToString();
            filteredBooks = string.IsNullOrWhiteSpace(searchTerm) ? books : books.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
        
        private async Task Logout()
        {
            await LocalStorage.RemoveItemAsync("jwtToken");
            NavigationManager.NavigateTo("/", true);
        }
    }
}