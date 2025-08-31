namespace DiscountCodes.Core.Models;

public sealed record GenerateResult(bool Success, IReadOnlyList<string> Codes);
