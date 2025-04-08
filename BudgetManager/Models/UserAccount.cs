using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace BudgetManager.Models;

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
}
