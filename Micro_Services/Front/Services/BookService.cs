using Entities;

namespace Front.Services
{
    public class BookService(HttpClient httpClient)
    {
        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            try
            {
                var response = await httpClient.GetAsync("http://localhost:5002/api/Book/");
                if (response.IsSuccessStatusCode)
                {
                    var books = await response.Content.ReadFromJsonAsync<IEnumerable<Book>>();
                    return books ?? new List<Book>();
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error : {ex}");
            }

            return new List<Book>();
        }

        public async Task<Book?> GetBookByIdAsync(int bookId)
        {
            try
            {
                var book = await httpClient.GetFromJsonAsync<Book>($"http://localhost:5002/api/Book/{bookId}");
                return book;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error : {ex}");
            }

            return null;
        }
    }
}