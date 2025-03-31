using System.Net;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.Sqlite;
using System.Text;

namespace PasswordHash;
public static class DatabaseCreator
{
    private static readonly string dataBase = "Data Source=Database.db";
        private const string packageDataBase = "Data Source=packageDataBase.db"; //Address for db

    public static void InitializeDatabase()
    {
        using (var connection = new SqliteConnection(dataBase))
        {
            CreateTableForUsers(connection);
            CreateMandatoryExpensesTable(connection);
            CreatePackageManagerTable();
            PackageManager.InstallSQLPackagesAndAddToDatabase(); //intaller for packages needed
        }
    }

    private static void CreateTableForUsers(SqliteConnection connection)
    {
        string command =
        "CREATE TABLE IF NOT EXISTS Users (id INTEGER PRIMARY KEY AUTOINCREMENT, username TEXT UNIQUE NOT NULL, passwordhash TEXT NOT NULL, salt TEXT NOT NULL)";


        var createUsersTableCommand = connection.CreateCommand();
        createUsersTableCommand.CommandText = command;

        connection.Open();
        createUsersTableCommand.ExecuteNonQuery();
        connection.Close();
    }


    //Method to create a mandatory expenses table if it does not exist - Метод для создания таблицы обязательных расходов, если она не существует
    private static void CreateMandatoryExpensesTable(SqliteConnection conn)
    {
        string command = @"
                    CREATE TABLE IF NOT EXISTS MandatoryExpenses (
                        userID INTEGER NOT NULL,
                        expenseID INTEGER PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL,
                        cost REAL NOT NULL
                    );
                ";

        using (var connection = new SqliteConnection(dataBase))
        {

            var createMandatoryExpensesTable = connection.CreateCommand();
            createMandatoryExpensesTable.CommandText = command;

            connection.Open();
            createMandatoryExpensesTable.ExecuteNonQuery();
            connection.Close();
        }
        //Copyright
        //*** This code was written by Olesia ***
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
}