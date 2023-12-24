using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entities;

namespace BookService.Data;

public class BookServiceContext : DbContext
{
    public BookServiceContext(DbContextOptions<BookServiceContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; } = default!;
}
