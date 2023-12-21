using Front.Services;
using Microsoft.AspNetCore.Components;

namespace Front.Components.Pages
{
    public partial class Explore : ComponentBase
    {
        [Inject]
        private BookService _BookService { get; set; }
        
        private IEnumerable<Book> books;
        private IEnumerable<Book> filteredBooks;
        private string searchTerm = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            books = await _BookService.GetBooksAsync();
            filteredBooks = books;
        }
        
        private void UpdateSearch(ChangeEventArgs e)
        {
            searchTerm = e.Value.ToString();
            filteredBooks = string.IsNullOrWhiteSpace(searchTerm) ? books : books.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
    }
}