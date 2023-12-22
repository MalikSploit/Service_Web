using Entities;
using Front.Services;
using Microsoft.AspNetCore.Components;

namespace Front.Components.Pages
{
    public partial class Explore : ComponentBase
    {
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        [Inject] private BookService BookService { get; set; }
        private IEnumerable<Book> books;
        private IEnumerable<Book> filteredBooks;
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

        private string searchTerm = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            books = await BookService.GetBooksAsync();
            filteredBooks = books;
        }
        
        private void UpdateSearch(ChangeEventArgs e)
        {
            searchTerm = e.Value!.ToString()!;
            filteredBooks = string.IsNullOrWhiteSpace(searchTerm) ? books : books.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
    }
}