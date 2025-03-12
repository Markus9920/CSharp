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
//CreateAccount -metodin siirto tähän luokkaan ja toiminnallisuuden muutokset(?) -ok(katto ny vielä!!)
//Login metodin siirto tähän luokkaan samanlailla niinkuin yllä mainittu -ok(katto ny vielä!!)
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
    private static bool CheckIfUsernameExists(string username) //checks if the username is already in use
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
    private static bool CheckPassword(string username, string passwordInput)
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
            connection.Close();
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
        }
        return true;
    }
    public static void DeleteUser()
    {
        int parsedID = 0;
        //Method to remove user from database.
        GetAllUsers(); //gets all the users from database

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
                DeleteUserByID(parsedID);
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
    private static bool DeleteUserByID(int ID)
    {
        using (var connection = new SqliteConnection(userDataBase))
        {
            connection.Open();
            var deleteUserCommand = connection.CreateCommand();
            deleteUserCommand.CommandText = "DELETE FROM Users WHERE id = $id";
            deleteUserCommand.Parameters.AddWithValue("$id", ID);
            int row = deleteUserCommand.ExecuteNonQuery(); //saves the integer from database to variable

            if (row > 0) //if integer returned from database is greater than zero, then it is actual id 
            {
                Console.WriteLine($"User with ID: {ID} deleted");
                connection.Close();
                return row > 0;
            }
            else //f not it is zero and not one or greater
            {
                Console.WriteLine($"User with ID: {ID} not found");
                connection.Close();
            }
        }
        return false;
    }
    //***These methods are not making SQL querys. These are just used inside query methods
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
                if (CheckIfUsernameExists(username))//if username is found, ask for password
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
                            if (CheckPassword(username, password))//if verification passed
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
            if (CheckIfUsernameExists(username))
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

        AddUserToDataBase(userAccount.Username, userAccount.Password, userAccount.Salt);
    }
}