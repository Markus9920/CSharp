using System.Net;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Data.Sqlite;


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
    public static void LookForUsername()
    {
        using (var connection = new SqliteConnection(userDataBase))
        {
            connection.Open();
            var lookForUsername = connection.CreateCommand();
            lookForUsername.CommandText = "";
        }
    }
}