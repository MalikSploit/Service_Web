using Entities;

namespace Front.Services;

public class BookService
{
    private readonly List<Book> books =
    [
        new Book
        {
            Id = 1,
            Title = "Cult of the Dead Cow",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/1.jpg",
            Price = 24.99m
        },
        new Book
        {
            Id = 2,
            Title = "The Cyber Effect",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/2.jpg",
            Price = 24.99m
        },
        new Book
        {
            Id = 3,
            Title = "Threat Modeling",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/3.jpg",
            Price = 24.99m
        },
        new Book
        {
            Id = 4,
            Title = "Social Engineering",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/4.jpg",
            Price = 24.99m
        },
        new Book
        {
            Id = 5,
            Title = "The Art Of Exploitation",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/5.jpg",
            Price = 24.99m
        },
        new Book
        {
            Id = 6,
            Title = "The CERT Guide",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/6.jpg",
            Price = 24.99m
        },
        new Book
        {
            Id = 7,
            Title = "Hacking Exposed",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/7.jpg",
            Price = 24.99m
        },
        new Book
        {
            Id = 8,
            Title = "Cybersecurity Fundamentals",
            Author = "Joseph Menn",
            Description = "An inside look at the hacker collective that changed cybersecurity.",
            ImageUrl = "Images/8.jpg",
            Price = 24.99m
        },
        new Book
        {
            Id = 9,
            Title = "Game Programming Patterns",
            Author = "Robert Nystrom",
            Description = "Game Programming Patterns is a collection of patterns I found in games that make code cleaner, easier to understand, and faster.",
            ImageUrl = "Images/9.jpg",
            Price = 32m
        },
        new Book
        {
            Id = 10,
            Title = "Design Patterns in .NET Core 3",
            Author = "Dmitri Nesteruk",
            Description = "Reusable Approaches in C# and F# for Object-Oriented Software Design.",
            ImageUrl = "Images/10.jpg",
            Price = 24m
        },
        new Book
        {
            Id = 11,
            Title = "Compilers Principles, Techniques, And Tools 2",
            Author = "Alfred V Aho, Monica S Lam, Ravi Sethi, Jeffrey D Ullman",
            Description = "One of the dragon book...",
            ImageUrl = "Images/11.jpg",
            Price = 100m
        },
        new Book
        {
            Id = 12,
            Title = "Compilers Principles, Techniques, And Tools",
            Author = "Alfred V Aho, Monica S Lam, Ravi Sethi, Jeffrey D Ullman",
            Description = "One of the dragon book...",
            ImageUrl = "Images/12.jpg",
            Price = 100m
        },
        new Book
        {
            Id = 13,
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
    
    public Task<Book?> GetBookByIdAsync(int bookId)
    {
        var book = books.FirstOrDefault(b => b.Id == bookId);
        return Task.FromResult(book);
    }
}
