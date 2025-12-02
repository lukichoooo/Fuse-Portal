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
}

