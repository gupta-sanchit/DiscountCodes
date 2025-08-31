using DiscountCodes.Abstractions.Services;
using System.Security.Cryptography;
using System.Text;
namespace DiscountCodesCryptography.Services;

internal class DiscountCodeGenerator : IDiscountCodeGenerator
{
    // 32 symbols: uppercase letters (no I/O) + digits 2–9
    private static readonly char[] Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray();

    public string Generate(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        // Fill cryptographically secure random bytes
        Span<byte> bytes = length <= 256 ? stackalloc byte[length] : new byte[length];
        RandomNumberGenerator.Fill(bytes);

        // Map uniformly into our 32-symbol alphabet
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
            sb.Append(Alphabet[bytes[i] % Alphabet.Length]);

        return sb.ToString();
    }
}
