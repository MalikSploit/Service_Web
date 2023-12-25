using System.Text.Json.Serialization;

namespace Entities;

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

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("author")]
    public string Author { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = "";

    [JsonPropertyName("price")]
    public decimal Price { get; set; } = decimal.MaxValue;

    public override string ToString() => $"\"{Title}\" by {Author}, available for {Price} euros";
}

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