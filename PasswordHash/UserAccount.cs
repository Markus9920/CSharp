using System.Net;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.Sqlite;
using System.Text;


//****TODO*****
//käyttäjätilin ja salasanan lisääminen tietokantaan -ok
//tarkastus onko käyttäjänimi jo käytössä -ok
//salasanan tarkastus -ok

//****Nice to have -osasto*****
//salasanan vaihtaminen
//kirjautumisyritykset x -kertaa


namespace PasswordHash;

public class UserAccount
{
    // Properties to store the username and hashed password
    public string Username { get; private set; }
    public string Password { get; private set; }
    public string Salt { get; private set; }

    //väliaikainen lista, johon olio tallennetaan
    List<UserAccount> userAccounts = new List<UserAccount>();

    // Generate a cryptographically secure random salt of the specified size
    byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
    string[] hashPasswordResult; //string array to store passwordhash and salt


    // Constants for salt size, key size, and iterations for PBKDF2 (Password-Based Key Derivation Function 2)
    const int saltSize = 16;  // Salt size in bytes (16 bytes is common)
    const int keySize = 64;   // Key size in bytes for the hash (64 bytes for SHA512) (Secure Hash Algorithm 512-bit)
    const int iterations = 350000;  // Number of iterations for PBKDF2 to increase security
    HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512; // Define the hash algorithm to be used (SHA-512 in this case)

    //constructor without properties. Just to get to use the methods.
    public UserAccount()
    {
        Username = String.Empty;
        Password = String.Empty;
        Salt = String.Empty;
        hashPasswordResult = new string[2];
    }

    // Constructor to create a new user account with username and password
    public UserAccount(string username, string password)
    {

        Username = username;
        hashPasswordResult = HashPassword(password);
        Password = hashPasswordResult[0];  // Assign the hash to Password
        Salt = hashPasswordResult[1];                // Assign the salt to Salt 
    }

    // Method to hash the password using PBKDF2 with SHA512 and a random salt
    private static string[] HashPassword(string password)
    {

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

        // Return the hashed password as a hexadecimal string
        return new string[] { Convert.ToHexString(hash), Convert.ToHexString(salt) };  // Return the password hash and the salt as hex string
    }
    public void CreateAccout()
    {
        string username = "";
        string password = "";
        //Query for username
        Console.WriteLine("Create user account");
        while (true)
        {
            Console.Write("Give username: ");
            username = Console.ReadLine() ?? String.Empty;

            if (username == String.Empty)
            {
                Console.WriteLine("Give valid username!");
                continue;
            }
            if (userAccounts.Any(user => user.Username == username))
            {
                Console.WriteLine("Username already exists! Please type another username.");
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

        Console.WriteLine($"Hashed password: {userAccount.Password}");
        Console.WriteLine($"Salt: {userAccount.Salt}");
        //userAccounts.Add(userAccount);
        //Console.WriteLine("Useraccount added into list.");
        UserDatabaseManager.AddUserToDataBase(userAccount.Username, userAccount.Password, userAccount.Salt);
    }
    //Method to verify the password
    private bool VerifyPassword(string password)
    {

        byte[] saltBytes = Convert.FromHexString(this.Salt);// Convert the stored salt (hex string) back into a byte array
        byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, iterations, hashAlgorithm, keySize);// Hash the input password using the stored salt

        // Compare the newly computed hash with the stored hash using a secure, time-resistant method
        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(Password));
    }
    private UserAccount FindUser(string username)
    {
        return userAccounts.FirstOrDefault(user => user.Username == username)!;
    }
    public void Login()
    {
        string username = "";
        string password = "";
        Console.WriteLine("Log in to user account");
        //Query for username
        while (true)
        {
            Console.Write("Enter username:");
            username = Console.ReadLine() ?? String.Empty;

            UserAccount user = FindUser(username);

            if (username == String.Empty)
            {
                Console.WriteLine("Empty input");
                continue;
            }
            else
            {
                if (user != null)//if username is found, ask for password
                {
                    Console.WriteLine("User found!");
                    //Query for password
                    while (true)
                    {
                        Console.WriteLine("Enter password:");
                        password = Console.ReadLine() ?? String.Empty;

                        if (username == String.Empty)
                        {
                            Console.WriteLine("Empty input");
                            continue;
                        }
                        else
                        {
                            if (user.VerifyPassword(password))//if verification passed
                            {
                                Console.WriteLine("Password correct!");
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Incorrect password!");
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }
}