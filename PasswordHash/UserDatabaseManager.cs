using System.Net;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Data.Sqlite;
using K4os.Compression.LZ4.Streams.Adapters;
using System.Data;
using Mysqlx.Resultset;


//*********TODO***********
//CreateAccount -metodista pitää tehdä uusi versio, joka toimii API kanssa yhteen
//Login -metodista pitää tehdä uusi versio, joka toimii API kanssa yhteen
//Luo käyttäjä -ok
//Hae kaikki käyttäjät -ok
//Hae käyttäjä id:llä
//päivitä käyttäjä
//poista käyttäjä -ok

//****Nice to have -osasto*****
//salasanan vaihtaminen
//kirjautumisyritykset x -kertaa

namespace PasswordHash;
public static class UserDatabaseManager
{
    private static readonly string userDataBase = "Data Source=userDatabase.db";

    public static bool AddUserToDataBase(string username, string password, string salt) //Adds user, passwordhash and salt to database
    {
        using (var connection = new SqliteConnection(userDataBase))
        {
            connection.Open();
            var addUserCommand = connection.CreateCommand();
            addUserCommand.CommandText = "INSERT INTO Users (username, passwordhash, salt) VALUES ($username, $passwordhash, $salt)";
            addUserCommand.Parameters.AddWithValue("$username", username);
            addUserCommand.Parameters.AddWithValue("$passwordhash", password);
            addUserCommand.Parameters.AddWithValue("$salt", salt);

            int row = addUserCommand.ExecuteNonQuery();

            connection.Close();

            return row > 0;
        }
    }

    public static bool CheckIfUsernameExists(string username) //checks if the username is already in use 
    {
        using (var connection = new SqliteConnection(userDataBase))
        {
            int count = 0;

            connection.Open();
            var checkIfUsernameExistsCommand = connection.CreateCommand();
            checkIfUsernameExistsCommand.CommandText = "SELECT COUNT(*) FROM Users WHERE username = $username";
            checkIfUsernameExistsCommand.Parameters.AddWithValue("$username", username);

            count = Convert.ToInt32(checkIfUsernameExistsCommand.ExecuteScalar());
            connection.Close();

            return count > 0;
        }
    }

    public static bool CheckPassword(string username, string passwordInput)
    //gets the password and salt based on username from database and makes comparison
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
            return false;    
        }
    }

    public static bool GetAllUsers()
    {
        //Gets all users and id's from database.
        using (var connection = new SqliteConnection(userDataBase))
        {
            connection.Open();
            var getAllUsersCommand = connection.CreateCommand();
            getAllUsersCommand.CommandText = "SELECT id, username FROM Users";

            var result = getAllUsersCommand.ExecuteReader();

            if (!result.HasRows)
            {
                return false;
            }

            while (result.Read())
            {

                Console.WriteLine($"ID: {result.GetInt32(0)} Username: {result.GetString(1)}");
            }
            connection.Close();
            return true;
        }     
    }

    public static bool DeleteUserByID(int ID)
    {
        using (var connection = new SqliteConnection(userDataBase))
        {
            connection.Open();
            var deleteUserCommand = connection.CreateCommand();
            deleteUserCommand.CommandText = "DELETE FROM Users WHERE id = $id";
            deleteUserCommand.Parameters.AddWithValue("$id", ID);
            int row = deleteUserCommand.ExecuteNonQuery(); //saves the integer from database to variable


            connection.Close();
            return row > 0; //true/false
        }
    }
}