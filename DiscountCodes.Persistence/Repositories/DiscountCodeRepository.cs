using DiscountCodes.Abstractions.Repositories;
using DiscountCodes.Persistence.Data;
using DiscountCodes.Persistence.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DiscountCodes.Persistence.Repositories;

internal class DiscountCodeRepository(DiscountDbContext discountDbContext) : IDiscountCodeRepository
{
    public async Task<bool> TryInsertAsync(string code, DateTimeOffset createdAt, CancellationToken ct)
    {
        discountDbContext.DiscountCodes.Add(new DiscountCode
        {
            Code = code,
            IsUsed = false,
            CreatedAt = createdAt,
            UsedAt = null
        });

        try
        {
            await discountDbContext.SaveChangesAsync(ct);
            return true;
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            // Clear the change tracker so the failed entity isn't left in Added state.
            discountDbContext.ChangeTracker.Clear();
            return false;
        }
    }
    public async Task<bool> ExistsAsync(string code, CancellationToken ct)
        => await discountDbContext.DiscountCodes.AsNoTracking().AnyAsync(c => c.Code == code, ct);

    public async Task<bool> TryConsumeAsync(string code, DateTimeOffset usedAt, CancellationToken ct)
    {
        var changed = await discountDbContext.DiscountCodes
            .Where(c => c.Code == code && !c.IsUsed)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.IsUsed, true)
                .SetProperty(c => c.UsedAt, usedAt),
                ct);

        return changed == 1;
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
       => ex.InnerException is SqliteException se && se.SqliteErrorCode == 19; // SQLITE_CONSTRAINT
}
