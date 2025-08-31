namespace DiscountCodes.Abstractions.Services;

/// <summary>
/// Provides functionality to generate discount codes of a specified length.
/// </summary>
public interface IDiscountCodeGenerator
{
    /// <summary>
    /// Generates a random discount code of the given length.
    /// </summary>
    /// <param name="length">The length of the discount code to generate.</param>
    /// <returns>A randomly generated discount code string.</returns>
    string Generate(int length);
}
