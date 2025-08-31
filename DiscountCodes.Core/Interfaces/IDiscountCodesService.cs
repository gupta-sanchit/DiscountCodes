using DiscountCodes.Abstractions.Enums;
using DiscountCodes.Core.Models;

namespace DiscountCodes.Core.Interfaces;

/// <summary>
/// Provides operations for generating and consuming discount codes.
/// </summary>
public interface IDiscountCodesService
{
    /// <summary>
    /// Generates a specified number of unique discount codes of a given length.
    /// </summary>
    /// <param name="count">The number of codes to generate (1-2000).</param>
    /// <param name="length">The length of each code (7 or 8 characters).</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A <see cref="GenerateResult"/> containing the outcome and generated codes.</returns>
    Task<GenerateResult> GenerateAsync(ushort count, byte length, CancellationToken ct = default);

    /// <summary>
    /// Attempts to consume a discount code, marking it as used if valid and unused.
    /// </summary>
    /// <param name="code">The discount code to consume.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A <see cref="UseCodeOutcome"/> indicating the result (success, already used, or invalid).</returns>
    Task<UseCodeOutcome> UseAsync(string code, CancellationToken ct = default);
}
