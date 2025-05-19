using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.Data.Sqlite;

namespace BudgetManager.Data
{
    public static class DatabaseCreator
    {
        private static readonly string dataBase = "Data Source = Database.db";
        private static readonly string packageDataBase = "Data Source = PackageDatabase.db";

        public static void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(dataBase))
            {
                CreateTableForUsers();
                CreateTableForCategories();
                CreateOtherExpensesTable();
                CreateRecurringExpensesTable();
                CreatePackageManagerTable();
                PackageManager.InstallPackages();
                CoolThing();

            }
        }

        private static void CreateTableForUsers()
        {
            string command =
            @"CREATE TABLE IF NOT EXISTS Users 
        (id INTEGER PRIMARY KEY AUTOINCREMENT, username TEXT UNIQUE NOT NULL, 
        passwordhash TEXT NOT NULL, salt TEXT NOT NULL)";

            using (var connection = new SqliteConnection(dataBase))
            {
                var createUsersTableCommand = connection.CreateCommand();
                createUsersTableCommand.CommandText = command;

                connection.Open();
                createUsersTableCommand.ExecuteNonQuery();
                connection.Close();
            }
        }

        private static void CreateTableForCategories()
        {

            string command =
            @"CREATE TABLE IF NOT EXISTS Categories
        (id INTEGER PRIMARY KEY, name TEXT UNIQUE NOT NULL)";

            using (var connection = new SqliteConnection(dataBase))
            {
                var createTableForCategories = connection.CreateCommand();
                createTableForCategories.CommandText = command;

                connection.Open();
                createTableForCategories.ExecuteNonQuery();
                connection.Close();

                CategoryManager.FillCategoriesTable();
            }
        }
        public static void CreatePackageManagerTable() //holds installed packages
        {


            const string command =
            "CREATE TABLE IF NOT EXISTS Packages (id INTEGER PRIMARY KEY AUTOINCREMENT, package_name TEXT UNIQUE NOT NULL)";

            using (var connection = new SqliteConnection(packageDataBase))
            {
                connection.Open();
                var createPackageManagerTable = connection.CreateCommand();
                createPackageManagerTable.CommandText = command;
                createPackageManagerTable.ExecuteNonQuery();
                connection.Close();
            }
        }

        //Method to create a mandatory expenses table if it does not exist - Метод для создания таблицы обязательных расходов, если она не существует
        private static void CreateOtherExpensesTable()
        {
            string command = @"
                CREATE TABLE IF NOT EXISTS OtherExpenses (
                expenseID INTEGER PRIMARY KEY AUTOINCREMENT,
                userID INTEGER NOT NULL,
                description TEXT NOT NULL,
                amount REAL NOT NULL,
                FOREIGN KEY (userID) REFERENCES Users(id)
            );
            ";
            using (var connection = new SqliteConnection(dataBase))
            {
                var createOtherExpensesTable = connection.CreateCommand();
                createOtherExpensesTable.CommandText = command;

                connection.Open();
                createOtherExpensesTable.ExecuteNonQuery();
                connection.Close();
            }
            //Copyright
            //*** This code was written by Konsta ***
        }

        private static void CreateRecurringExpensesTable()
        {
            string command = @"
            CREATE TABLE IF NOT EXISTS RecurringExpenses (
                expenseID INTEGER PRIMARY KEY AUTOINCREMENT,
                userID INTEGER NOT NULL,
                description TEXT NOT NULL,
                cost REAL NOT NULL,
                occurenceDate TEXT NOT NULL,
                recurrenceType TEXT NOT NULL,  -- Например, 'Monthly' или 'Yearly'
                nextOccurrence TEXT NOT NULL,   -- Дата следующего наступления (в формате 'yyyy-MM-dd')
                isPaid BOOLEAN NOT NULL DEFAULT 0,
                category TEXT
            );
        ";
            using (var connection = new SqliteConnection(dataBase))
            {
                var createRecurringExpensesTable = connection.CreateCommand();
                createRecurringExpensesTable.CommandText = command;

                connection.Open();
                createRecurringExpensesTable.ExecuteNonQuery();
                connection.Close();
            }
            //Copyright
            //*** This code was written by Olesia -edited by Markus, added occurenceDate ***
        }
        private static void CoolThing()
        {
            Console.WriteLine("Establishing database.");
            for (int i = 0; i < 25; i++)
            {
                Console.Write("#");
                Thread.Sleep(20);
            }
            Console.WriteLine("\nDatabase established.");
        }
    }
}