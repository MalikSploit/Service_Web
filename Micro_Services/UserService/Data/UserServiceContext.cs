using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entities;

namespace UserService.Data;


/* 
    * Class : UserServiceContext
    * -----------------------
    * This class is the context for the UserService database. It contains a
    * DbSet of User objects.
*/
public class UserServiceContext : DbContext
{
    public UserServiceContext (DbContextOptions<UserServiceContext> options)
        : base(options)
    {
    }

    public DbSet<User> User { get; set; } = default!;
}
