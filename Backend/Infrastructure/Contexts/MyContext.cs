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
            .WithMany(u => u.UserUniversities)
            .HasForeignKey(uu => uu.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);

        // -------- User

        mb.Entity<Chat>()
            .HasOne(c => c.User)
            .WithMany(u => u.Chats)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Chat → Messages 
        mb.Entity<Message>()
            .HasOne(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        // Message → ChatFiles 
        mb.Entity<ChatFile>()
            .HasOne(cf => cf.Message)
            .WithMany(m => m.Files)
            .HasForeignKey(cf => cf.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---- Portal ------
        mb.Entity<Subject>()
            .HasOne(s => s.User)
            .WithMany(u => u.Subjects)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Subject → Schedules
        mb.Entity<Schedule>()
            .HasOne(sch => sch.Subject)
            .WithMany(s => s.Schedules)
            .HasForeignKey(sch => sch.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Subject → Lecturers
        mb.Entity<Lecturer>()
            .HasOne(l => l.Subject)
            .WithMany(s => s.Lecturers)
            .HasForeignKey(l => l.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Subject → Tests
        mb.Entity<Test>()
            .HasOne(t => t.Subject)
            .WithMany(s => s.Tests)
            .HasForeignKey(t => t.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}

