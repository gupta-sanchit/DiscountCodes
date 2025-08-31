using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DiscountCodes.Persistence.Data;

public sealed class DiscountDbContextFactory : IDesignTimeDbContextFactory<DiscountDbContext>
{
    public DiscountDbContext CreateDbContext(string[] args)
    {
        //we can remove below once we decide to go for any enterprise db (postgres, sql server)
        var solutionRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ".."));
        var dbDir = Path.Combine(solutionRoot, "db");
        Directory.CreateDirectory(dbDir);
        var dbPath = Path.Combine(dbDir, "discount.db");
        var connectionString = $"Data Source={dbPath};Cache=Shared";
        var opts = new DbContextOptionsBuilder<DiscountDbContext>()
            .UseSqlite(connectionString)
            .Options;

        return new DiscountDbContext(opts);
    }
}
