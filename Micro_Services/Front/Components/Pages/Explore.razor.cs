using Blazored.LocalStorage;
using Entities;
using Front.Services;
using Microsoft.AspNetCore.Components;

namespace Front.Components.Pages
{
    public partial class Explore : ComponentBase
    {
        #pragma warning disable CS8618
        [Inject] private BookService BookService { get; set; }
        [Inject] private ILocalStorageService LocalStorage { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private LoginService LoginService { get; set; }
        
        private IEnumerable<Book> books;
        private IEnumerable<Book> filteredBooks;
        private bool _isLoggedIn;
        private string? searchTerm = string.Empty;
        private bool isDropdownOpen;

        protected override async Task OnInitializedAsync()
        {
            books = await BookService.GetBooksAsync();
            filteredBooks = books;
            _isLoggedIn = await LoginService.IsUserLoggedIn();
        }
        
        private void UpdateSearch(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString();
            filteredBooks = string.IsNullOrWhiteSpace(searchTerm) ? books : books.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
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
}