
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class Context : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=Fuse-Test;User Id=sa;Password=lUKA_2006LUKA;TrustServerCertificate=True;");
    }

    public DbSet<User> Users { get; set; }
    public DbSet<University> Universities { get; set; }
}

