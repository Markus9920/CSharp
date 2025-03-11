using System.Net;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Data.Sqlite;
using K4os.Compression.LZ4.Streams.Adapters;


namespace PasswordHash;
public static class UserDatabaseManager
{
    private static readonly string userDataBase = "Data Source=userDatabase.db";

    public static void AddUserToDataBase(string username, string password, string salt) //Add user and passwordhash to database
    {
        using (var connection = new SqliteConnection(userDataBase))
        {
            connection.Open();
            var addUserCommand = connection.CreateCommand();
            addUserCommand.CommandText = "INSERT INTO Users (username, passwordhash, salt) VALUES ($username, $passwordhash, $salt)";
            addUserCommand.Parameters.AddWithValue("$username", username);
            addUserCommand.Parameters.AddWithValue("$passwordhash" , password);
            addUserCommand.Parameters.AddWithValue("$salt", salt);
            addUserCommand.ExecuteNonQuery();
            connection.Close();
        }
    }
    public static bool CheckIfUsernameExists(string username)
    {
        using (var connection = new SqliteConnection(userDataBase))
        {
            int count = 0;

            connection.Open();
            var checkIfUsernameExistsCommand = connection.CreateCommand();
            checkIfUsernameExistsCommand.CommandText = "SELECT COUNT(*) FROM Users WHERE username = $username";
            checkIfUsernameExistsCommand.Parameters.AddWithValue("$username", username);

            count = Convert.ToInt32(checkIfUsernameExistsCommand.ExecuteScalar());
            return count > 0;
        }
    }
    public static bool CheckPassword(string username, string passwordInput)
    {
        using (var connection = new SqliteConnection(userDataBase))
        {
            string passwordHashFromDataBase = "";
            string saltFromDataBase = "";

            int iterations = 350000;
            int keySize = 64;
           

            connection.Open();
            var checkPasswordCommand = connection.CreateCommand();
            checkPasswordCommand.CommandText = "SELECT passwordhash, salt FROM Users WHERE username = $username";
            checkPasswordCommand.Parameters.AddWithValue("$username", username);
            
            var result = checkPasswordCommand.ExecuteReader();

            while (result.Read())
            {
                //Saving the data from database to variables
                passwordHashFromDataBase = result.GetString(0);
                saltFromDataBase = result.GetString(1);

                byte[] salt = Convert.FromHexString(saltFromDataBase); // Convert stored salt from hex string to byte array

                // Hash the input password using the stored salt
                byte[] hashForComparing = Rfc2898DeriveBytes.Pbkdf2(passwordInput, salt, iterations, HashAlgorithmName.SHA512, keySize);

                connection.Close();
                // Compare the new hash with the hash from database
                return CryptographicOperations.FixedTimeEquals(hashForComparing, Convert.FromHexString(passwordHashFromDataBase)); 
            }
            connection.Close();
            return false;
        }   
    }
}