using Entities;

namespace Front.Services;

public class BookService(HttpClient httpClient)
{
    public async Task<IEnumerable<Book>> GetBooksAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("http://localhost:5000/api/Book/");
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
            var book = await httpClient.GetFromJsonAsync<Book>($"http://localhost:5000/api/Book/{bookId}");
            return book;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error : {ex}");
        }

        return null;
    }
        
    public async Task<bool> DeleteBookAsync(int bookId)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"http://localhost:5000/api/Book/{bookId}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error during book deletion: {ex.Message}");
            return false;
        }
    }
    
    public async Task<Book?> AddBookAsync(Book book)
    {
        var response = await httpClient.PostAsJsonAsync("http://localhost:5000/api/Book", book);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Book>();
        }
        return null;
    }
    
    public async Task<bool> UpdateBookAsync(Book book)
    {
        var response = await httpClient.PutAsJsonAsync($"http://localhost:5000/api/Book/{book.Id}", book);
        return response.IsSuccessStatusCode;
    }
    
}