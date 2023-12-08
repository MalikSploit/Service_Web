using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using UserLib;

namespace API;

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
    // On déclare que notre base aura une table Utilisateurs qui contiendra des
    public DbSet<Utilisateur> Utilisateurs { get; set; }
}
// API.csproj