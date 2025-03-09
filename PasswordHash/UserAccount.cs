using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace PasswordHash;

public class UserAccount
{
    // Properties to store the username and hashed password
    public string Username { get; private set; }
    public string Password { get; private set; }



    // Constants for salt size, key size, and iterations for PBKDF2 (Password-Based Key Derivation Function 2)
    const int saltSize = 16;  // Salt size in bytes (16 bytes is common)
    const int keySize = 64;   // Key size in bytes for the hash (64 bytes for SHA512) (Secure Hash Algorithm 512-bit)
    const int iterations = 350000;  // Number of iterations for PBKDF2 to increase security

    //constructor without properties. Just to get use of the methods.
    public UserAccount()
    {
        Username = String.Empty;
        Password = String.Empty;
    }

    // Constructor to create a new user account with username and password
    public UserAccount(string username, string password)
    {
        Username = username;  // Assign the provided username
        Password = HashPassword(password);  // Hash the password and store it
    }

    // Method to hash the password using PBKDF2 with SHA512 and a random salt
    private static string HashPassword(string password)
    {
        // Define the hash algorithm to be used (SHA-512 in this case)
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        // Generate a cryptographically secure random salt of the specified size
        byte[] salt = RandomNumberGenerator.GetBytes(saltSize);

        // Use PBKDF2 to hash the password with the salt
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),  // Convert the password to bytes using UTF-8 encoding
            salt,                             // Use the generated salt
            iterations,                       // The number of iterations to apply for added security
            hashAlgorithm,                    // The hashing algorithm (SHA-512)
            keySize);                         // The size of the resulting hash (64 bytes)

        // Print the hashed password (hexadecimal format) and salt (hexadecimal format)
        Console.WriteLine($"Password hash: {Convert.ToHexString(hash)}");  // Print the password hash in hex format
        Console.WriteLine($"Generated salt: {Convert.ToHexString(salt)}");  // Print the salt in hex format

        // Return the hashed password as a hexadecimal string
        return Convert.ToHexString(hash);  // Return the password hash as a hex string
    }
    public void CreateAccout()
    {
        string username = "";
        string password = "";
        //Query for username
        while (true)
        {
            Console.Write("Give username: ");
            username = Console.ReadLine() ?? String.Empty;

            if (username == String.Empty)
            {
                Console.WriteLine("Give valid username!");
                continue;
            }
            Console.WriteLine();
            break;
        }

        //Query for password
        while (true)
        {
            Console.Write("Give password: ");
            password = Console.ReadLine() ?? String.Empty;

            if (password == String.Empty)
            {
                Console.WriteLine("Give valid password");
                continue;
            }
            Console.WriteLine();
            break;
        }
        //Creates new account
        UserAccount userAccount = new UserAccount(username, password);
    }
}