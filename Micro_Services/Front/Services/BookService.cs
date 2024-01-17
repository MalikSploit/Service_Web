using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Entities;
using Microsoft.AspNetCore.Components;

namespace Front.Services;

public class BookService(HttpClient httpClient, ILocalStorageService localStorage, LoginService loginService)
{
    [Inject] private ILocalStorageService LocalStorage { get; set; } = localStorage;

    private async Task<bool> IsUserAdmin()
    {
        return await loginService.IsUserAdmin();
    }
    
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
        
    public async Task DeleteBookAsync(int bookId)
    {
        try
        {
            var jwtToken = await LocalStorage.GetItemAsStringAsync("jwtToken");
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new InvalidOperationException("User is not logged in.");
            }
            if (!await IsUserAdmin())
            {
                throw new UnauthorizedAccessException("Only admins can delete books.");
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            await httpClient.DeleteAsync($"http://localhost:5000/api/Book/{bookId}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error during book deletion: {ex.Message}");
        }
    }
    
    public async Task<Book?> AddBookAsync(Book book)
    {
        var jwtToken = await LocalStorage.GetItemAsStringAsync("jwtToken");
        if (string.IsNullOrEmpty(jwtToken))
        {
            throw new InvalidOperationException("User is not logged in.");
        }
        if (!await IsUserAdmin())
        {
            throw new UnauthorizedAccessException("Only admins can delete books.");
        }

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await httpClient.PostAsJsonAsync("http://localhost:5000/api/Book", book);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Book>();
        }
        return null;
    }
    
    public async Task<bool> UpdateBookAsync(Book book)
    {
        var jwtToken = await LocalStorage.GetItemAsStringAsync("jwtToken");
        if (string.IsNullOrEmpty(jwtToken))
        {
            throw new InvalidOperationException("User is not logged in.");
        }
        if (!await IsUserAdmin())
        {
            throw new UnauthorizedAccessException("Only admins can delete books.");
        }

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await httpClient.PutAsJsonAsync($"http://localhost:5000/api/Book/{book.Id}", book);
        return response.IsSuccessStatusCode;
    }
}