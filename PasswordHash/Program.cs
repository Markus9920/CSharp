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
        DatabaseCreator.InitializeDatabase(); //Method that establishes database and creates tables needed.
        PackageManager.InstallSQLPackagesAndAddToDatabase(); //Method that installs all packages needed, if not installed (not found from db)


        Console.WriteLine("Use console(1)? or API(2)?");
        if (Console.ReadLine() == "1")
        {
            ConsoleApp.RunProgram();
        }
        else
        {
            Console.WriteLine("No API yet!");
        }
    }
}
