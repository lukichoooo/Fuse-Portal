using Core.Entities;
using Core.Entities.Convo;
using Core.Entities.JoinTables;
using Core.Entities.Portal;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class MyContext(DbContextOptions<MyContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<University> Universities { get; set; }

    public DbSet<UserUniversity> UserUniversities { get; set; }

    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<Lecturer> Lecturers { get; set; }

    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatFile> ChatFiles { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);


        // UserUnniversity

        mb.Entity<UserUniversity>()
            .HasKey(uu => new { uu.UserId, uu.UniversityId });

        mb.Entity<UserUniversity>()
            .HasOne(uu => uu.User)
            .WithMany(u => u.UserUniversities)
            .HasForeignKey(uu => uu.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<UserUniversity>()
            .HasOne(uu => uu.University)
            .WithMany(uni => uni.UserUniversities)
            .HasForeignKey(uu => uu.UniversityId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}

