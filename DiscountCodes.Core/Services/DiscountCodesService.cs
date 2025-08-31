using DiscountCodes.Abstractions.Enums;
using DiscountCodes.Abstractions.Repositories;
using DiscountCodes.Abstractions.Services;
using DiscountCodes.Core.Interfaces;
using DiscountCodes.Core.Models;
using Microsoft.Extensions.Logging;

namespace DiscountCodes.Core.Services;

public sealed class DiscountCodesService(IDiscountCodeRepository discountCodeRepository, 
    IDiscountCodeGenerator discountCodeGenerator, ILogger<DiscountCodesService> logger) : IDiscountCodesService
{
    // 32-symbol allowed alphabet: no I/O/0/1 to reduce confusion
    private static readonly HashSet<char> Allowed =
        [.. "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray()];

    public async Task<GenerateResult> GenerateAsync(ushort count, byte length, CancellationToken ct = default)
    {
        // input validation
        if (length is < 7 or > 8 || count is 0 or > 2000)
        {
            logger.LogWarning("Invalid input for GenerateAsync: count= {Count}, length= {Length}", count, length);
            return new GenerateResult(false, []);
        }

        var created = new List<string>(count);

        // Loop until 'count' unique rows are actually persisted
        while ((ushort)created.Count < count)
        {
            ct.ThrowIfCancellationRequested();

            logger.LogDebug("Generating code {Current}/{Total}", created.Count + 1, count);
            
            var code = Normalize(discountCodeGenerator.Generate(length));
            if (!IsValid(code, length))
            {
                logger.LogWarning("Generated code failed validation: {Code}", code);
                continue; // defensive; generator should already comply
            }

            // Try to insert; on UNIQUE(Code) conflict, just generate another
            if (await discountCodeRepository.TryInsertAsync(code, DateTimeOffset.UtcNow, ct))
            {
                created.Add(code);
            }
        }

        return new GenerateResult(true, created);
    }

    public async Task<UseCodeOutcome> UseAsync(string code, CancellationToken ct = default)
    {
        code = Normalize(code);
        if (!IsValid(code, code.Length))
        {
            logger.LogWarning("Invalid code provided to UseAsync: {Code}", code);
            return UseCodeOutcome.Invalid;
        }

        // Atomic one-time consume (only flips if currently unused)
        if (await discountCodeRepository.TryConsumeAsync(code, DateTimeOffset.UtcNow, ct))
        {
            return UseCodeOutcome.Success;
        }

        // Distinguish "already used" vs "never existed"
        var exists = await discountCodeRepository.ExistsAsync(code, ct);
        if (exists)
        {
            return UseCodeOutcome.AlreadyUsed;
        }
        else
        {
            logger.LogWarning("Code does not exist: {Code}", code);
            return UseCodeOutcome.Invalid;
        }
    }

    private static string Normalize(string s) => (s ?? string.Empty).Trim().ToUpperInvariant();

    private static bool IsValid(string code, int length)
        => (length is 7 or 8)
           && code.Length == length
           && code.All(Allowed.Contains);
}
