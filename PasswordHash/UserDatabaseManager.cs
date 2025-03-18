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
using System.ComponentModel.Design;
using Org.BouncyCastle.Asn1.Misc;


//*********TODO***********
//CreateAccount -metodista pitää tehdä uusi versio, joka toimii API kanssa yhteen
//Login -metodista pitää tehdä uusi versio, joka toimii API kanssa yhteen
//Luo käyttäjä -ok
//Hae kaikki käyttäjät -ok
//Hae käyttäjä id:llä
//päivitä käyttäjä
//poista käyttäjä -ok
//GetAllUsers pitää palauttaa lista (Dto) APIa varten!!!!!! -ok

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

            try
            {
                int row = addUserCommand.ExecuteNonQuery();
                return row > 0;
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                connection.Close();
            }


        }
    }

    public static int Authentication(string username, string passwordInput)
    //gets the password and salt based on username from database and makes comparison
    {
        using (var connection = new SqliteConnection(userDataBase))
        {
            //Variables to store data from database 
            string passwordHashFromDataBase = "";
            string saltFromDataBase = "";
            int userId = 0;

            //variables to use in password salting and hashing
            int iterations = 350000;
            int keySize = 64;

            connection.Open();
            var checkPasswordCommand = connection.CreateCommand();
            checkPasswordCommand.CommandText = "SELECT id, passwordhash, salt FROM Users WHERE username = $username";
            checkPasswordCommand.Parameters.AddWithValue("$username", username);

            var result = checkPasswordCommand.ExecuteReader();

            if (result.Read())
            {
                //Saving the data from database to variables
                userId = result.GetInt32(0);//id
                passwordHashFromDataBase = result.GetString(1);//hash from db
                saltFromDataBase = result.GetString(2);//passeord salt from db

                byte[] salt = Convert.FromHexString(saltFromDataBase); // Convert stored salt from hex string to byte array

                // Hash the input password using the stored salt
                byte[] hashForComparing = Rfc2898DeriveBytes.Pbkdf2(passwordInput, salt, iterations, HashAlgorithmName.SHA512, keySize);



                // Compare the new hash with the hash from database, if correct return userId
                if (CryptographicOperations.FixedTimeEquals(hashForComparing, Convert.FromHexString(passwordHashFromDataBase)))
                {
                    connection.Close();
                    return userId;
                }

            }
            connection.Close();
            return 0;
        }
    }

    public static List<UserDTo> GetAllUsers()
    {
        List<UserDTo> users = new List<UserDTo>();
        //Gets all users and id's from database.
        using (var connection = new SqliteConnection(userDataBase))
        {
            connection.Open();
            var getAllUsersCommand = connection.CreateCommand();
            getAllUsersCommand.CommandText = "SELECT id, username FROM Users ORDER BY id";

            var result = getAllUsersCommand.ExecuteReader();

            while (result.Read()) //adds all data into list as an object
            {
                users.Add(new UserDTo(result.GetInt32(0), result.GetString(1)));
            }

            foreach(var user in users)
            {
                Console.WriteLine($"{user.Id} {user.Username}");
            }
            connection.Close();
            return users; //returns list

        }
    }

    public static bool DeleteUserByID(int Id)
    {
        using (var connection = new SqliteConnection(userDataBase))
        {
            connection.Open();
            var deleteUserCommand = connection.CreateCommand();
            deleteUserCommand.CommandText = "DELETE FROM Users WHERE id = $id";
            deleteUserCommand.Parameters.AddWithValue("$id", Id);
            int row = deleteUserCommand.ExecuteNonQuery(); //saves the integer from database to variable


            connection.Close();
            return row > 0; //true/false
        }
    }
}