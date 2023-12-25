using Microsoft.EntityFrameworkCore;
using Entities;

namespace BookService.Data;

public class BookServiceContext(DbContextOptions<BookServiceContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; } = default!;
}
