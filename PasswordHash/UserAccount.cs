using System.Net;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.Sqlite;
using System.Text;

namespace PasswordHash;

public class UserAccount
{
    // Properties to store the username and hashed password
    public string Username { get; private set; }
    public string Password { get; private set; }

    public string Salt { get; private set; }

    // Generate a cryptographically secure random salt of the specified size
    byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
    string[] hashPasswordResult; //string array to store passwordhash and salt


    // Constants for salt size, key size, and iterations for PBKDF2 (Password-Based Key Derivation Function 2)
    const int saltSize = 16;  // Salt size in bytes (16 bytes is common)
    const int keySize = 64;   // Key size in bytes for the hash (64 bytes for SHA512) (Secure Hash Algorithm 512-bit)
    const int iterations = 350000;  // Number of iterations for PBKDF2 to increase security
    HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512; // Define the hash algorithm to be used (SHA-512 in this case)


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
        return [Convert.ToHexString(hash), Convert.ToHexString(salt)];  // Return the password hash and the salt as hex string in array
    } 
    public static void Login()
    {
        string username = "";
        string password = "";
        Console.WriteLine("Log in to user account");
        //Query for username
        while (true)
        {
            Console.Write("Enter username:");
            username = Console.ReadLine() ?? String.Empty;

            if (username == String.Empty)
            {
                Console.WriteLine("Empty input");
                continue;
            }
            else
            {
                if (UserDatabaseManager.CheckIfUsernameExists(username))//if username is found, ask for password
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
                            if (UserDatabaseManager.CheckPassword(username, password))//if verification passed
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
                    break;
                }
            }
        }
    }
    public static void CreateAccout()
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
            if (UserDatabaseManager.CheckIfUsernameExists(username))
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

        UserDatabaseManager.AddUserToDataBase(userAccount.Username, userAccount.Password, userAccount.Salt);
    }
     public static void DeleteUser()
    {
        int parsedID = 0;
        //Method to remove user from database.
        UserDatabaseManager.GetAllUsers(); //gets all the users from database

        while (true)
        {
            Console.WriteLine("Give ID for deleting user");
            string id = Console.ReadLine() ?? String.Empty;

            if (id == String.Empty || !int.TryParse(id, out parsedID)) //if id is empty, or it cannot be parsed to int
            {
                Console.WriteLine("Give valid ID!");
            }
            break;
        }
        Console.WriteLine($"ID: {parsedID}");

        while (true)
        {
            Console.WriteLine($"Are you sure you want to delete user with ID: {parsedID} Y/N?");
            string answer = Console.ReadLine() ?? String.Empty;
            if (answer == "Y" || answer == "y")
            {
                UserDatabaseManager.DeleteUserByID(parsedID);
                break;
            }
            else if (answer == String.Empty)
            {
                Console.WriteLine("Invalid input");
                continue;
            }
            else
            {
                Console.WriteLine("Cancel user delete");
                break;
            }
        }

    }
}