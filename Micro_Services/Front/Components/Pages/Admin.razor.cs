using System.Globalization;
using Blazored.LocalStorage;
using Entities;
using Front.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace Front.Components.Pages;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public partial class Admin : ComponentBase
{
    #pragma warning disable CS8618
    [Inject] protected BookService BookService { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] private ILocalStorageService LocalStorage { get; set; }
    [Inject] private LoginService LoginService { get; set; }
    [Inject] private CartStateService CartStateService { get; set; }
    
    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationStateTask { get; set; }

    private IEnumerable<Book> Books;
    private bool IsLoading = true;
    private Book SelectedBook = new();
    private bool ShowEditModal;
    private bool ShowDeleteConfirmModal;
    private bool isDropdownOpen;
    private bool _isUserAdmin;
    private string modalTitle = "";
    private string ErrorMessageUrl = string.Empty;
    private string ErrorMessageImage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _isUserAdmin = await LoginService.IsUserAdmin();
        if (_isUserAdmin)
        {
            await LoadBooks();
            IsLoading = false;
        }
        else
        {
            NavigationManager.NavigateTo("/", true);
        }
    }

    private async Task LoadBooks()
    {
        Books = await BookService.GetBooksAsync();
    }

    private void ShowAddBookForm()
    {
        SelectedBook = new Book();
        ShowEditModal = true;
        SelectedBook.Price = 0;
        modalTitle = "Create Book";
    }

    private async Task DeleteBook(int id)
    {
        await BookService.DeleteBookAsync(id);
        await LoadBooks();
    }

    private async Task ConfirmDelete(int bookId)
    {
        // Retrieve the book to get the image URL
        var bookToDelete = Books.FirstOrDefault(b => b.Id == bookId);
        if (bookToDelete == null)
        {
            return;
        }
        
        await DeleteBook(bookId);

        // Delete the image file if the book has an image
        if (!string.IsNullOrEmpty(bookToDelete.ImageUrl))
        {
            DeleteImage(bookToDelete.ImageUrl);
        }

        // Close the modal and reload the book list
        CloseDeleteConfirmModal();
        await LoadBooks();
    }
    
    private static string ConvertUrlToFilePath(string imageUrl)
    {
        var imagePath = Path.Combine("wwwroot", imageUrl.TrimStart('/'));
        return imagePath;
    }
    
    private static void DeleteImage(string imageUrl)
    {
        try
        {
            var imagePath = ConvertUrlToFilePath(imageUrl);
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error : {ex.Message}");
        }
    }
    
    private static bool IsValidImageUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        var allowedExtensions = new[] { ".jpg", ".png", ".jpeg", "webp" };
        var urlLower = url.ToLowerInvariant();
        return allowedExtensions.Any(ext => urlLower.EndsWith(ext));
    }

    private async Task SaveBook()
    {
        bool operationSuccessful;
        ErrorMessageUrl = string.Empty;

        if (!IsValidImageUrl(SelectedBook.ImageUrl))
        {
            ErrorMessageUrl = "Invalid image URL. Please enter a valid URL to an image file.";
            return;
        }

        if (SelectedBook.Id == 0) // Adding a new book
        {
            var newBook = await BookService.AddBookAsync(SelectedBook);
            operationSuccessful = newBook != null;
        }
        else // Updating an existing book
        {
            operationSuccessful = await BookService.UpdateBookAsync(SelectedBook);
        }

        if (operationSuccessful)
        {
            await LoadBooks();
            CloseEditModal();

            if (SelectedBook.Id == 0) // If it was a new book, reset SelectedBook
            {
                SelectedBook = new Book();
            }
        }
    }
    
    private string PriceString
    {
        get => SelectedBook.Price.ToString(CultureInfo.InvariantCulture);
        set => SelectedBook.Price = decimal.TryParse(value, CultureInfo.InvariantCulture, out var result) ? result : 0;
    }

    private void OpenEditModal(Book book)
    {
        SelectedBook = book;
        ShowEditModal = true;
        modalTitle = "Edit Book";
    }

    private void CloseEditModal()
    {
        ShowEditModal = false;
    }

    private void OpenDeleteConfirmModal(Book book)
    {
        SelectedBook = book;
        ShowDeleteConfirmModal = true;
    }

    private void CloseDeleteConfirmModal()
    {
        ShowDeleteConfirmModal = false;
    }
    
    private async Task HandleImageUpload(InputFileChangeEventArgs e)
    {
        const long maxFileSize = 5 * 1024 * 1024; // 5 MB
        var allowedExtensions = new[] { ".jpg", ".png", ".jpeg", "webp" };
        var imageDirectoryPath = Path.Combine("wwwroot", "Images");
        var imageFile = e.File;

        try
        {
            if (imageFile.Size > maxFileSize)
            {
                throw new InvalidOperationException("File size is too large. Maximum allowed size is 5 MB.");
            }

            var fileExtension = Path.GetExtension(imageFile.Name).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Invalid file type.");
            }

            var maxImageNumber = GetMaxImageNumber(imageDirectoryPath);
            var newImageName = $"{maxImageNumber + 1}{fileExtension}";
            var path = Path.Combine(imageDirectoryPath, newImageName);

            await using (var stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.OpenReadStream(maxFileSize).CopyToAsync(stream);
            }

            SelectedBook.ImageUrl = $"/Images/{newImageName}";
        }
        catch (Exception ex)
        {
            ErrorMessageImage = ex.Message;
        }
    }

    private static int GetMaxImageNumber(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var imageFiles = Directory.EnumerateFiles(directoryPath);
        var maxNumber = 0;

        foreach (var file in imageFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            if (int.TryParse(fileName, out var fileNumber))
            {
                maxNumber = Math.Max(maxNumber, fileNumber);
            }
        }

        return maxNumber;
    }

    
    private string GetDropdownClass()
    {
        return isDropdownOpen ? "block z-10" : "hidden";
    }

    private void ToggleDropdown()
    {
        isDropdownOpen = !isDropdownOpen;
    }
    
    private async Task Logout()
    {
        await LocalStorage.RemoveItemAsync("jwtToken");
        await CartStateService.ClearCartAsync();
        NavigationManager.NavigateTo("/", true);
    }
}