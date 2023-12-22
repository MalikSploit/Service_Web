using System.Diagnostics;

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

    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Description { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public decimal Price { get; set; } = decimal.MaxValue;

    public override string ToString() => " : \"" + Title.ToString() + "\" par " + Author.ToString() + "dispo pour " + Price.ToString() + " euro";
}

public class BookCreate : BookWithoutID
{
    public BookCreate(string title, string author, string description, string imageUrl, decimal price) : base(title, author, description, imageUrl, price) { }
    public BookCreate() { }
}

public class Book : BookWithoutID
{
    public Book() { }
    public Book(int id, string title, string author, string description, string imageUrl, decimal price) : base(title, author, description, imageUrl, price)
    {
        Id = id;
    }

    public int Id { get; set; } = 0;
    public override string ToString() => "Livre #" + Id + " : \"" + Title.ToString() + "\" par " + Author.ToString() + "dispo pour " + Price.ToString() + " euro";
}
