using System.Net;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.Sqlite;
using System.Text;
using Microsoft.OpenApi.Validations;

namespace PasswordHash;
public static class DatabaseCreator
{
    private const string userDataBase = "Data Source=userDatabase.db"; //Address for db
    private const string packageDataBase = "Data Source=packageDataBase.db"; //Address for db
    public static void InitializeDatabase()
    {
        using (var connection = new SqliteConnection(userDataBase))
        {
            connection.Open();
            Console.WriteLine("\nEstablishing SQL database");

            for (int i = 0; i < 20; i++) //cool loading effect
            {
                Console.Write("*"); 
                Thread.Sleep(50);
            }

            Console.WriteLine();
            CreateTableForUsers();
            CreatePackageManagerTable();
            Console.WriteLine("Establishing SQL database completed\n");
            connection.Close();
        }
    }

    private static void CreateTableForUsers()
    {
        using (var connection = new SqliteConnection(userDataBase))
        {
            const string command =
           "CREATE TABLE IF NOT EXISTS Users (id INTEGER PRIMARY KEY AUTOINCREMENT, username TEXT UNIQUE NOT NULL, passwordhash TEXT NOT NULL, salt TEXT NOT NULL)";

            connection.Open();
            var createUsersTableCommand = connection.CreateCommand();
            createUsersTableCommand.CommandText = command;
            createUsersTableCommand.ExecuteNonQuery();
            connection.Close();
        }
    }
    public static void CreatePackageManagerTable() //holds installed packages
    {
        using (var connection = new SqliteConnection(packageDataBase))
        {
            const string command =
            "CREATE TABLE IF NOT EXISTS Packages (id INTEGER PRIMARY KEY AUTOINCREMENT, package_name TEXT UNIQUE NOT NULL)";

            connection.Open();
            var createPackageManagerTable = connection.CreateCommand();
            createPackageManagerTable.CommandText = command;
            createPackageManagerTable.ExecuteNonQuery();
            connection.Close();
        }
    }
    //other table creation functionality
}