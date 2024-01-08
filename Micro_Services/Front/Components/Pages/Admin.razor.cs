using System.Globalization;
using Entities;
using Front.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace Front.Components.Pages;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public partial class Admin : ComponentBase
{
    [Inject] protected BookService BookService { get; set; }

    private IEnumerable<Book> Books;
    private bool IsLoading = true;
    private Book SelectedBook = new();
    private bool ShowEditModal;
    private bool ShowDeleteConfirmModal;

    protected override async Task OnInitializedAsync()
    {
        await LoadBooks();
        IsLoading = false;
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
    }

    private async Task DeleteBook(int id)
    {
        await BookService.DeleteBookAsync(id);
        await LoadBooks();
    }

    private async Task ConfirmDelete(int bookId)
    {
        await DeleteBook(bookId);
        CloseDeleteConfirmModal(); // Close the modal after confirming deletion
    }

    private async Task SaveBook()
    {
        bool operationSuccessful;

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
        set => SelectedBook.Price = decimal.TryParse(value, out var result) ? result : 0;
    }

    private void OpenEditModal(Book book)
    {
        SelectedBook = book;
        ShowEditModal = true;
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
        var imageFile = e.File;
        {
            // Get the highest number from existing images
            var imageDirectoryPath = Path.Combine("wwwroot", "Images");
            var maxImageNumber = GetMaxImageNumber(imageDirectoryPath);

            // Create a new image name
            var newImageName = $"{maxImageNumber + 1}{Path.GetExtension(imageFile.Name)}";
            var path = Path.Combine(imageDirectoryPath, newImageName);

            await using (var stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.OpenReadStream().CopyToAsync(stream);
            }

            // Update the ImageUrl of the SelectedBook
            SelectedBook.ImageUrl = $"/Images/{newImageName}";
        }
    }

    private static int GetMaxImageNumber(string directoryPath)
    {
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
}