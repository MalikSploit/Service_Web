using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Entities;

/* 
    * Class : BookWithoutID
    * -----------------------
    * This class is the model for a Book object. It contains properties for
    * the title, author, description, image URL, and price of a book.
*/
public class BookWithoutID
{
    public BookWithoutID() { }
    public BookWithoutID(string title, string author, string description, string imageUrl, decimal price)
    {
        Title = title;
        Author = author;
        Description = description;
        ImageUrl = imageUrl;
        Price = price;
    }

    [Required(ErrorMessage = "Title is required")]
    [MaxLength(100, ErrorMessage = "Title is too long")]
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Author is required")]
    [MaxLength(100, ErrorMessage = "Author name is too long")]
    [JsonPropertyName("author")]
    public string Author { get; set; } = "";

    [Required(ErrorMessage = "Description is required")]
    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [Required(ErrorMessage = "Image URL is required")]
    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = "";

    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
    [JsonPropertyName("price")]
    public decimal Price { get; set; } = decimal.MaxValue;

    public override string ToString() => $"\"{Title}\" by {Author}, available for {Price} euros";
}


/* 
    * Class : Book
    * -----------------------
    * This class is the model for a Book object. It contains properties for
    * the ID, title, author, description, image URL, and price of a book.
    * It inherits from BookWithoutID and adds an ID property.
*/
public class Book : BookWithoutID
{
    public Book() { }
    public Book(int id, string title, string author, string description, string imageUrl, decimal price) : base(title, author, description, imageUrl, price)
    {
        Id = id;
    }

    [JsonPropertyName("id")]
    public int Id { get; set; } = 0;

    public override string ToString() => $"Book #{Id}: \"{Title}\" by {Author}, available for {Price} euros";
}