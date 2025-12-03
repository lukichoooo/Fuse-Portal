using Core.Entities;
using Core.Entities.Convo;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class MyContext(DbContextOptions<MyContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<University> Universities { get; set; }

    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatFile> ChatFiles { get; set; }
    public DbSet<Message> Messages { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Many-to-many User <-> University
        modelBuilder.Entity<User>()
            .HasMany(u => u.Universities)
            .WithMany(u => u.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UniversityUser",
                j => j
                    .HasOne<University>()
                    .WithMany()
                    .HasForeignKey("UniversitiesId")
                    .OnDelete(DeleteBehavior.NoAction),
                j => j
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UsersId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("UniversitiesId", "UsersId");
                }
            );
    }
}

