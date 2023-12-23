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
        private IEnumerable<Book> books;
        private IEnumerable<Book> filteredBooks;

        private bool _isLoggedIn;
        private string searchTerm = string.Empty;
        private bool isDropdownOpen;

        protected override async Task OnInitializedAsync()
        {
            books = await BookService.GetBooksAsync();
            filteredBooks = books;
            _isLoggedIn = await IsUserLoggedIn();
        }

        private async Task<bool> IsUserLoggedIn()
        {
            var jwtToken = await LocalStorage.GetItemAsStringAsync("jwtToken");
            return !string.IsNullOrEmpty(jwtToken);
        }

        private struct SearchScore 
        {
            public int Score;
            public Book Livre;

            public SearchScore(int score, Book b)
            {
                Score = score;
                this.Livre = b;
            }
        }

        private void UpdateSearch(ChangeEventArgs e)
        {
            searchTerm = e.Value!.ToString()!.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(searchTerm)) 
            {
                filteredBooks = books;
            }
            var terms = searchTerm.Split();

            List<SearchScore> s = books.Select(b =>
                new SearchScore(b.Title.Split(' ').Select(s => StringDistance.LevenshteinDistance(s.ToLowerInvariant(), searchTerm)).Min(), b)
                ).Where(s=>s.Score <= 4).ToList();
            s.Sort((a, b) => a.Score.CompareTo(b.Score));

            filteredBooks = string.IsNullOrWhiteSpace(searchTerm) ? books : s.Select(a => a.Livre);
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