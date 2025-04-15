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
    private string Password;
    private string Salt;

    //getters
    public string GetPass() => Password;
    public string GetSalt() => Salt;


    // Constants for salt size, key size, and iterations for PBKDF2 (Password-Based Key Derivation Function 2)
    const int saltSize = 16;  // Salt size in bytes (16 bytes is common)
    const int keySize = 64;   // Key size in bytes for the hash (64 bytes for SHA512) (Secure Hash Algorithm 512-bit)
    const int iterations = 350000;  // Number of iterations for PBKDF2 to increase security

    // Constructor to create a new user account with username and password
    public UserAccount(string username, string password)
    {
        string trimmedPassword = password.Trim();
        var (hash, salt) = HashPassword(trimmedPassword);

        Username = username.Trim();
        Password = Convert.ToHexString(hash);  // Assign the hash to Password
        Salt = Convert.ToHexString(salt);                // Assign the salt to Salt 
    }

    // Method to hash the password using PBKDF2 with SHA512 and a random salt
    private static (byte[] Hash, byte[] Salt) HashPassword(string password)
    {
        password = password.Trim();

        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        // Generate a cryptographically secure random salt of the specified size
        byte[] salt = GenerateSalt();

        // Use PBKDF2 to hash the password with the salt
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),  // Convert the password to bytes using UTF-8 encoding
            salt,                             // Use the generated salt
            iterations,                       // The number of iterations to apply for added security
            hashAlgorithm,                    // The hashing algorithm (SHA-512)
            keySize);                         // The size of the resulting hash (64 bytes)

        // Print the hashed password (hexadecimal format) and salt (hexadecimal format)

        // Return the hashed password as a hexadecimal string
        return (hash, salt);  // Return the password hash and the salt as hex string in array
    }

    private static byte[] GenerateSalt() //this method generates random byte[] with 16 bytes
    {
        byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
        return salt;
    }
    public static bool Verify(string oldPassword, string storedHash, string storedSalt)
    {

        
        //this method verifies if typed password is correct
        string trimmedPassword = oldPassword.Trim();

        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        if (String.IsNullOrWhiteSpace(oldPassword))
        {
            return false;
        }


        byte[] originalHash = Convert.FromHexString(storedHash);

        byte[] salt = Convert.FromHexString(storedSalt);

        byte[] hashForComparing = Rfc2898DeriveBytes.Pbkdf2(
        Encoding.UTF8.GetBytes(trimmedPassword),
        salt,
        iterations,
        hashAlgorithm,
        keySize);

        return CryptographicOperations.FixedTimeEquals(hashForComparing, originalHash);

    }

    //This returns new password
    public static (byte[] Hash, byte[] Salt) NewPassword(string newPassword) => HashPassword(newPassword);
}
