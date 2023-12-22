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
        
    ];

    public Task<IEnumerable<Book>> GetBooksAsync()
    {
        return Task.FromResult<IEnumerable<Book>>(books);
    }
}
