using Front.Services;
using Microsoft.AspNetCore.Components;

namespace Front.Components.Pages
{
    public partial class Explore : ComponentBase
    {
        [Inject]
        private BookService _BookService { get; set; }
        
        private IEnumerable<Book> books;

        protected override async Task OnInitializedAsync()
        {
            books = await _BookService.GetBooksAsync();
        }
    }
}