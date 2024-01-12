using Microsoft.EntityFrameworkCore;
using Entities;

namespace BookService.Data;

/*
    * Class: BookServiceContext
    * -----------------------
    * This class is the context for the BookService database. It contains a
    * DbSet of Book objects.
*/
public class BookServiceContext(DbContextOptions<BookServiceContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; } = default!;
}
