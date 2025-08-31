using DiscountCodes.Abstractions.Repositories;
using DiscountCodes.Persistence.Data;
using DiscountCodes.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscountCodes.Persistence.Registrations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        //we can remove below once we decide to go for any enterprise db (postgres, sql server)
        var solutionRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ".."));
        var dbDir = Path.Combine(solutionRoot, "db");
        Directory.CreateDirectory(dbDir);
        var dbPath = Path.Combine(dbDir, "discount.db");
        var connectionString = $"Data Source={dbPath};Cache=Shared";

        services.AddDbContext<DiscountDbContext>(options =>
        options.UseSqlite(connectionString));

        services.AddScoped<IDiscountCodeRepository, DiscountCodeRepository>();

        return services;
    }
}