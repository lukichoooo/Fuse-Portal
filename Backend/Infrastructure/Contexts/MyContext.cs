using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Contexts;

public class MyContext(DbContextOptions<MyContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<University> Universities { get; set; }
}

