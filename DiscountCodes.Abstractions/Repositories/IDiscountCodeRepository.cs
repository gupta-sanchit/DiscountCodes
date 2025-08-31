namespace DiscountCodes.Abstractions.Repositories;

/// <summary>
/// Repository interface for discount code persistence and usage operations.
/// </summary>
public interface IDiscountCodeRepository
{
    /// <summary>
    /// Attempts to insert a new discount code into persistent storage.
    /// </summary>
    /// <param name="code">The discount code to insert.</param>
    /// <param name="createdAt">The creation timestamp.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>True if the code was inserted; false if it already exists.</returns>
    Task<bool> TryInsertAsync(string code, DateTimeOffset createdAt, CancellationToken ct);

    /// <summary>
    /// Checks if a discount code exists in persistent storage.
    /// </summary>
    /// <param name="code">The discount code to check.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>True if the code exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(string code, CancellationToken ct);

    /// <summary>
    /// Attempts to consume (mark as used) a discount code if it is valid and unused.
    /// </summary>
    /// <param name="code">The discount code to consume.</param>
    /// <param name="usedAt">The timestamp when the code is used.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>True if the code was successfully consumed; otherwise, false.</returns>
    Task<bool> TryConsumeAsync(string code, DateTimeOffset usedAt, CancellationToken ct);
}
