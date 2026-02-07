using System.Security.Cryptography;
using System.Text;

using SkillMap.Business.Abstractions;

namespace SkillMap.Infrastructure.Account;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128-bit
    private const int HashSize = 32; // 256-bit
    private const int Iterations = 10000; // Number of PBKDF2 iterations

    /// <summary>
    /// Hashes a password using a deterministic salt derived from the password itself.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>A hashed password in the format {salt}:{hash}.</returns>
    public string Hash(string password)
    {
        // Generate a deterministic salt
        byte[] salt = GenerateDeterministicSalt(password);

        // Hash the password with the deterministic salt
        byte[] hash = PBKDF2(password, salt);

        // Combine salt and hash into a single string
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Verifies a password against a hashed password.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>True if the password is valid; otherwise, false.</returns>
    public bool Verify(string password, string hashedPassword)
    {
        // Split the hashed password into salt and hash
        var parts = hashedPassword.Split(':');
        if (parts.Length != 2)
            return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] storedHash = Convert.FromBase64String(parts[1]);

        // Hash the password with the same salt
        byte[] hash = PBKDF2(password, salt);

        // Compare the hashes
        return CryptographicOperations.FixedTimeEquals(hash, storedHash);
    }

    /// <summary>
    /// Generates a deterministic salt based on the password itself.
    /// </summary>
    /// <param name="password">The password to base the salt on.</param>
    /// <returns>A deterministic salt.</returns>
    private byte[] GenerateDeterministicSalt(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            // Use the SHA-256 hash of the password as the salt
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password)).AsSpan(0, SaltSize).ToArray();
        }
    }

    /// <summary>
    /// Hashes a password using PBKDF2 with a given salt.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="salt">The salt to use.</param>
    /// <returns>A byte array containing the hash.</returns>
    private byte[] PBKDF2(string password, byte[] salt)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
        {
            return pbkdf2.GetBytes(HashSize);
        }
    }
}