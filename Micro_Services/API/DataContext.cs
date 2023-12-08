using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using UserLib;

namespace Api;

public class DataContext : DbContext
{
    protected readonly IConfiguration Configuration;
    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Connexion a la base sqlite
        options.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));
    }

    public DbSet<Utilisateur> Utilisateurs { get; set; }
}