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
    private static readonly string userDataBase = "Data Source=userDatabase.db";

    public static void InitializeDatabase()
    {
        using (var connection = new SqliteConnection(userDataBase))
        {
            Console.WriteLine("\nEstablishing SQL database");          
            CreateTableForUsers(connection);
            Console.WriteLine("Establishing SQL database completed\n");
            PackageManager.InstallSQLPackages();


        }
    }
    static void CreateTableForUsers(SqliteConnection conn)
    {
        conn.Open();
        var createUsersTableCommand = conn.CreateCommand();
        createUsersTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS Users (id INTEGER PRIMARY KEY, username TEXT UNIQUE NOT NULL, passwordhash TEXT NOT NULL, salt TEXT NOT NULL)";
        createUsersTableCommand.ExecuteNonQuery();
        conn.Close();
    }
    //other table creation functionality
}