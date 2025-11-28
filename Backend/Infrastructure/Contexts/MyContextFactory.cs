using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using Core.Dtos.Settings;

namespace Infrastructure.Contexts;

public class MyContextFactory : IDesignTimeDbContextFactory<MyContext>
{
    public MyContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=BankingSystem;User Id=sa;Password=lUKA_LOCAL;TrustServerCertificate=True;");

        return new MyContext(optionsBuilder.Options);
    }
}

