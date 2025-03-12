using System.Net;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Data.Sqlite;
namespace PasswordHash;

class Program
{
    static void Main(string[] args)
    {
        UserAccount userAccount = new UserAccount(); //this must be called, because it establishes database connection and hadles the package installation
        DatabaseCreator.InitializeDatabase(); //Method that establishes database and creates tables needed.


        while (true)
        {
            Console.WriteLine("(1) Create account (2) Login (3) Get users from databas (4) Delete user from database");
            string input = Console.ReadLine() ?? String.Empty;

            if (input == String.Empty)
            {
                Console.WriteLine("Empty input!");
                continue;
            }
            switch (input)
            {
                case "1":
                    UserDatabaseManager.CreateAccout();
                    break;
                case "2":
                    UserDatabaseManager.Login();
                    break;
                case"3":
                    UserDatabaseManager.GetAllUsers();
                    break;
                case"4":
                    UserDatabaseManager.DeleteUser();
                    break;        
                default:
                Console.WriteLine("Give valid input");  
                break; 
            }
        }

    }
}
