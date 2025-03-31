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
using Org.BouncyCastle.Tls;


//*********TODO***********
//Luo käyttäjä -ok
//Hae kaikki käyttäjät -ok
//Hae käyttäjä id:llä -ok
//päivitä käyttäjä -ok (salasana)
//poista käyttäjä -ok
//GetAllUsers pitää palauttaa lista (Dto) APIa varten!!!!!! -ok

//****Nice to have -osasto*****
//salasanan vaihtaminen
//kirjautumisyritykset x -kertaa

//muistio:
/*Ohjelman alussa luodaan CreateMandatoryExpensesTable metodilla uusi taulu, johon käyttäjien pakolliset menot lisätään. Käyttäjä itse lisää pakolliset menot tauluun.

Tauluissa olevat olevat menot pitää pystyä laskemaan yhteen metodilla ja näyttämään käyttäjälle.

*/

namespace PasswordHash;
public static class DatabaseManager
{
    private const int saltSize = 16;//Used for salting and hashing new password
    private const int iterations = 350000;//Used for salting and hashing new password
    private const int keySize = 64;//Used for salting and hashing new password


    private static readonly string dataBase = "Data Source=dataBase.db";//Address for db

    private static string[] HashPassword(string password) //hashes password before adding to db
    {
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        byte[] salt = RandomNumberGenerator.GetBytes(saltSize); //hae suola tietokannasta!!!!!!

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            hashAlgorithm,
            keySize);

        return [Convert.ToHexString(hash), Convert.ToHexString(salt)];
    }

    //**** Methods for users table******
    public static bool AddUserToDataBase(string username, string password, string salt) // add new user to database, if not existing
    {
        string command =
        "INSERT INTO Users (username, passwordhash, salt) VALUES ($username, $passwordhash, $salt)";

        try
        {
            using (var connection = new SqliteConnection(dataBase))
            {

                var addUserCommand = connection.CreateCommand();
                addUserCommand.CommandText = command;
                addUserCommand.Parameters.AddWithValue("$username", username);
                addUserCommand.Parameters.AddWithValue("$passwordhash", password);
                addUserCommand.Parameters.AddWithValue("$salt", salt);

                connection.Open();
                int row = addUserCommand.ExecuteNonQuery();
                connection.Close();

                return row > 0; //affected row greater than zero means its true
            }
        }
        catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067) // UNIQUE constraint error
        {
            return false; //false, user already exists
        }
    }

    public static int Authentication(string username, string passwordInput) //loggin in, checks password and username
    {
        string command =
        "SELECT id, passwordhash, salt FROM Users WHERE username = $username";

        using (var connection = new SqliteConnection(dataBase))
        {
            int userId = -1;//stays this way if reader does not find anything

            int iterations = 350000;
            int keySize = 64;


            var checkPasswordCommand = connection.CreateCommand();
            checkPasswordCommand.CommandText = command;
            checkPasswordCommand.Parameters.AddWithValue("$username", username);

            connection.Open();
            using (var result = checkPasswordCommand.ExecuteReader())
            {
                if (result.Read())
                {
                    userId = result.GetInt32(0);
                    string passwordHashFromDatabase = result.GetString(1);
                    string saltFromDatabase = result.GetString(2);

                    //creates hash from given password and compares to password in db
                    byte[] salt = Convert.FromHexString(saltFromDatabase);
                    byte[] hashForComparing = Rfc2898DeriveBytes.Pbkdf2(passwordInput, salt, iterations, HashAlgorithmName.SHA512, keySize);

                    if (CryptographicOperations.FixedTimeEquals(hashForComparing, Convert.FromHexString(passwordHashFromDatabase)))
                    {
                        return userId; //return userid if found
                    }
                    else
                    {
                        return -1;//wrong password
                    }
                }
            }
            connection.Close();
            return -1; //return -1 if nothing found
        }
    }

    public static List<UserDTO> GetAllUsers() //get all users from bd and put into list of Data Transfer Objects
    {
        string command = "SELECT id, username FROM Users ORDER BY id";

        List<UserDTO> users = new List<UserDTO>();

        using (var connection = new SqliteConnection(dataBase))
        {

            var getAllUsersCommand = connection.CreateCommand();
            getAllUsersCommand.CommandText = command;

            connection.Open();
            using (var result = getAllUsersCommand.ExecuteReader())
            {
                while (result.Read())
                {
                    users.Add(new UserDTO(result.GetInt32(0), result.GetString(1))); //add to list
                }
            }

            connection.Close();
            return users; //return list, empty if nothing is found
        }
    }

    public static UserDTO? GetUserId(int id) //finds user by id number
    {
        string command = "SELECT id, username FROM Users WHERE id = $id";

        using (var connection = new SqliteConnection(dataBase))
        {
            connection.Open();
            var getUserIdCommand = connection.CreateCommand();
            getUserIdCommand.CommandText = command;
            getUserIdCommand.Parameters.AddWithValue("$id", id);

            using (var result = getUserIdCommand.ExecuteReader())
            {
                if (result.Read())
                {
                    return new UserDTO(result.GetInt32(0), result.GetString(1)); //returns new Data Transfer Object
                }
            }

            connection.Close();
            return null; //otherwise returns null
        }
    }

    public static bool DeleteUserById(int Id) //delete user by id
    {
        string command = "DELETE FROM Users WHERE id = $id";

        using (var connection = new SqliteConnection(dataBase))
        {

            var deleteUserCommand = connection.CreateCommand();
            deleteUserCommand.CommandText = command;
            deleteUserCommand.Parameters.AddWithValue("$id", Id);

            connection.Open();
            int row = deleteUserCommand.ExecuteNonQuery();
            connection.Close();

            return row > 0; //affected row greater than zero means its true
        }
    }

    public static bool ChangePassword(string username, string oldPassword, string newPassword)
    {

        string command =
        "UPDATE Users SET passwordhash = $passwordhash, salt = $salt WHERE id = $id";

        //Changes old password to new one and hashes it
        int userId = Authentication(username, oldPassword);

        if (userId < 0)
        {
            return false;
        }

        string[] newPass = HashPassword(newPassword);//array of hash and salt
        string newPassHash = newPass[0];
        string newPassSalt = newPass[1]; //tee metodi, joka hakee tietokannasta suolan!!!!

        using (var connection = new SqliteConnection(dataBase))
        {

            var changePasswordCommand = connection.CreateCommand();
            changePasswordCommand.CommandText = command;
            changePasswordCommand.Parameters.AddWithValue("$passwordhash", newPassHash);
            changePasswordCommand.Parameters.AddWithValue("$salt", newPassSalt);
            changePasswordCommand.Parameters.AddWithValue("$id", userId);

            connection.Open();
            int rows = changePasswordCommand.ExecuteNonQuery();
            connection.Close();

            return rows > 0; //affected row greater than zero means its true
        }
    }



    //********Methods for expendses tables********

    public static double SumMandatoryExpenses(int userId) //sums all expenses from table
    {
        string command =
        "SELECT SUM(cost) FROM MandatoryExpenses WHERE userID = $userId";
        double totalExpenseCost = 0;

        using (var connection = new SqliteConnection(dataBase))
        {
            var sumMandatoryExpensesCommand = connection.CreateCommand();
            sumMandatoryExpensesCommand.CommandText = command;
            sumMandatoryExpensesCommand.Parameters.AddWithValue("$userId", userId);

            connection.Open();
            var result = sumMandatoryExpensesCommand.ExecuteScalar();
            connection.Close();
            // If the result is not null, convert it to double; otherwise, use 0 as the default value
            return totalExpenseCost = result != null ? Convert.ToDouble(result) : 0;
        }
    }

    //Method for adding a new mandatory expense - Метод для добавления нового обязательного расхода
    public static bool AddExpense(int userId, string name, double cost)
    {
        using (var connection = new SqliteConnection(dataBase))
        {

            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO MandatoryExpenses (userID, name, cost) VALUES ($userID, $name, $cost)";
            command.Parameters.AddWithValue("$userID", userId);
            command.Parameters.AddWithValue("$name", name);
            command.Parameters.AddWithValue("$cost", cost);

            connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            connection.Close();

            return rowsAffected > 0;
        }
        //Copyright
        //*** This code was written by Olesia ***
    }
    public static DataTable GetExpensesForUser(int userId)
    {
        DataTable dt = new DataTable();
        using (var connection = new SqliteConnection(dataBase))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT expenseID, name, cost FROM MandatoryExpenses WHERE userID = $userID";
            command.Parameters.AddWithValue("$userID", userId);
            using (var reader = command.ExecuteReader())
            {
                dt.Load(reader);
            }
            connection.Close();
        }
        return dt;

        //Copyright
        //*** This code was written by Olesia ***
    }


    //Method for updating mandatory expense record - Метод для обновления записи обязательного расхода
    public static bool UpdateExpense(int expenseId, int userId, string name, double cost)
    {
        using (var connection = new SqliteConnection(dataBase))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                    UPDATE MandatoryExpenses 
                    SET name = $name, cost = $cost 
                    WHERE expenseID = $expenseID AND userID = $userID
                ";
            command.Parameters.AddWithValue("$name", name);
            command.Parameters.AddWithValue("$cost", cost);
            command.Parameters.AddWithValue("$expenseID", expenseId);
            command.Parameters.AddWithValue("$userID", userId);
            int rowsAffected = command.ExecuteNonQuery();
            connection.Close();
            return rowsAffected > 0;
        }

        //Copyright
        //*** This code was written by Olesia ***
    }

    //Method for removing mandatory expense - Метод для удаления обязательного расхода
    public static bool DeleteExpense(int expenseId, int userId)
    {
        using (var connection = new SqliteConnection(dataBase))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM MandatoryExpenses WHERE expenseID = $expenseID AND userID = $userID";
            command.Parameters.AddWithValue("$expenseID", expenseId);
            command.Parameters.AddWithValue("$userID", userId);
            int rowsAffected = command.ExecuteNonQuery();
            connection.Close();
            return rowsAffected > 0;
        }

        //Copyright
        //*** This code was written by Olesia ***
    }

}