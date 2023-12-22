using Entities;

namespace Front.Services;

public class BookService
{
    private readonly List<Book> books =
    [
        new Book
        {
            Title = "Cult of the Dead Cow",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/1.jpg",
            Price = 24.99m
        },
        new Book
        {
            Title = "The Cyber Effect",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/2.jpg",
            Price = 24.99m
        },
        new Book
        {
            Title = "Threat Modeling",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/3.jpg",
            Price = 24.99m
        },
        new Book
        {
            Title = "Social Engineering",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/4.jpg",
            Price = 24.99m
        },
        new Book
        {
            Title = "The Art Of Exploitation",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/5.jpg",
            Price = 24.99m
        },
        new Book
        {
            Title = "The CERT Guide",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/6.jpg",
            Price = 24.99m
        },
        new Book
        {
            Title = "Hacking Exposed",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/7.jpg",
            Price = 24.99m
        },
        new Book
        {
            Title = "Cybersecurity Fundamentals",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/8.jpg",
            Price = 24.99m
        },
        new Book
        {
            Title = "Game Programming Patterns",
            Author = "Robert Nystrom",
            Description = "Game Programming Patterns is a collection of patterns I found in games that make code cleaner, easier to understand, and faster.",
            ImageUrl = "Images/9.jpg",
            Price = 32m
        },
        new Book
        {
            Title = "Design Patterns in .NET Core 3",
            Author = "Dmitri Nesteruk",
            Description = "Reusable Approaches in C# and F# for Object-Oriented Software Design.",
            ImageUrl = "Images/10.jpg",
            Price = 24m
        },
        new Book
        {
            Title = "Compilers Principles, Techniques, And Tools 2",
            Author = "Alfred V Aho, Monica S Lam, Ravi Sethi, Jeffrey D Ullman",
            Description = "One of the dragon book...",
            ImageUrl = "Images/11.jpg",
            Price = 100m
        },
        new Book
        {
            Title = "Compilers Principles, Techniques, And Tools",
            Author = "Alfred V Aho, Monica S Lam, Ravi Sethi, Jeffrey D Ullman",
            Description = "One of the dragon book...",
            ImageUrl = "Images/12.jpg",
            Price = 100m
        },
        new Book
        {
            Title = "Compilers Principles, Techniques, And Tools",
            Author = "Alfred V Aho, Monica S Lam, Ravi Sethi, Jeffrey D Ullman",
            Description = "One of the dragon book...",
            ImageUrl = "Images/13.jpg",
            Price = 100m
        },
    ];

    public Task<IEnumerable<Book>> GetBooksAsync()
    {
        return Task.FromResult<IEnumerable<Book>>(books);
    }
}
