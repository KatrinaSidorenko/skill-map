using System.Text;
using System.Security.Cryptography;

using Microsoft.Extensions.Options;

using SkillMap.Business.Abstractions;
using SkillMap.Shared.Options;
using Konscious.Security.Cryptography;

namespace SkillMap.Infrastructure.Account;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHashOptions _options;
    public PasswordHasher(IOptions<PasswordHashOptions> options)
    {
        _options = options.Value;
    }

    private byte[] CreateSalt() => RandomNumberGenerator.GetBytes(_options.SaltSize);
    private string HashPasswordWithSalt(string password, byte[] salt)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        using var argon2 = new Argon2id(passwordBytes)
        {
            Salt = salt,
            DegreeOfParallelism = _options.DegreeOfParallelism,
            Iterations = _options.Iterations,
            MemorySize = _options.MemorySize
        };
        var hashBytes = argon2.GetBytes(_options.HashSize);
        return Convert.ToHexString(hashBytes);
    }

    private string ComposeHashString(byte[] salt, string hash) => $"{Convert.ToHexString(salt)}{_options.Separator}{hash}";
    private string[] DecomposeHash(string hashedPassword) => hashedPassword.Split(_options.Separator);

    public string Hash(string password)
    {
        var salt = CreateSalt();
        var hash = HashPasswordWithSalt(password, salt);
        return ComposeHashString(salt, hash);
    }

    public bool Verify(string password, string hashedPassword)
    {
        var parts = DecomposeHash(hashedPassword);
        if (parts.Length != 2) return false;

        var salt = Convert.FromHexString(parts[0]);
        var hash = parts[1];

        var computedHash = HashPasswordWithSalt(password, salt);
        return computedHash == hash;
    }
}