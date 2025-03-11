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
            Console.WriteLine("(1) Create account (2) Login");
            string input = Console.ReadLine() ?? String.Empty;

            if (input == String.Empty)
            {
                Console.WriteLine("Empty input!");
                continue;
            }
            switch (input)
            {
                case "1":
                    userAccount.CreateAccout();
                    break;
                case "2":
                    userAccount.Login();
                    break;
                default:
                Console.WriteLine("Give valid input");  
                Console.WriteLine("(1) Create account (2) Login (3) Install SQL packages");
                break; 
            }
        }

    }
}
