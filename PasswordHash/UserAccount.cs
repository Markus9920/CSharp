using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace PasswordHash;

public class UserAccount
{
    public string Username { get; private set; }
    public string Password { get; private set; }


    const int saltSize = 16;
    const int keySize = 64;
    const int iterations = 350000;


    public UserAccount(string username, string password)
    {
        Username = username;
        Password = HashPassword(password);
    }

    private static string HashPassword(string password)
    {
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        byte[] salt = RandomNumberGenerator.GetBytes(saltSize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
        Encoding.UTF8.GetBytes(password),
        salt,
        iterations,
        hashAlgorithm,
        keySize);

        Console.WriteLine($"Password hash: {Convert.ToHexString(hash)}");
        Console.WriteLine($"Generated salt: {Convert.ToHexString(salt)}");

        return Convert.ToHexString(hash);
    }
}