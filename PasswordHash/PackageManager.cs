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



namespace PasswordHash;
public static class PackageManager
{
    private const string packageDataBase = "Data Source=packageDataBase.db";//Address for db

    public static void InstallSQLPackagesAndAddToDatabase() //intaller for packages needed
    {
        Console.WriteLine("**********Installing packages. This may take several minutes**********");

        string sqlite = "Microsoft.Data.Sqlite";
        string entityFrameworkCore = "Microsoft.EntityFrameworkCore.Sqlite";
        string swagger = "Swashbuckle.AspNetCore";
        string swaggerGen = "Swashbuckle.AspNetCore.SwaggerGen";
        string swaggerUI = "Swashbuckle.AspNetCore.SwaggerUI";
 
        List<string> packages = new List<string> { sqlite, entityFrameworkCore, swagger, swaggerGen, swaggerUI }; //saves package names to list
        List<string> installedPackages = GetPackagesFromDataBase(); //list for installed packages

        foreach (var package in packages)
        {
            if (!installedPackages.Contains(package)) //if not in installed packages list, then its not found from database
            {
                Process.Start("dotnet", $"add package {package}")?.WaitForExit();
                AddPackageToDataBase(package); //adds the package to database, so it is "marked" as installed
            }
            else
            {
                Console.WriteLine($"Package {package} already installed");
            }
        }

        Console.WriteLine("\n**********Package installation complete*********\n");
    }

    public static void AddPackageToDataBase(string packageName) //adds package to database
    {
        const string command =
        "INSERT INTO Packages (package_name) VALUES ($packageName)";
        using (var connection = new SqliteConnection(packageDataBase))
        {
            connection.Open();
            var addPackageToDataBase = connection.CreateCommand();
            addPackageToDataBase.CommandText = command;
            addPackageToDataBase.Parameters.AddWithValue("$packageName", packageName);
            addPackageToDataBase.ExecuteNonQuery();
            connection.Close();
        }
    }

    public static List<string> GetPackagesFromDataBase() //gets all packages from database
    {
        const string command = "SELECT package_name FROM Packages";

        List<string> installedPackages = new List<string>();

        using (var connection = new SqliteConnection(packageDataBase))
        {
            connection.Open();
            var getPackagesFromDataBaseCommand = connection.CreateCommand();
            getPackagesFromDataBaseCommand.CommandText = command;

            using (var reader = getPackagesFromDataBaseCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    installedPackages.Add(reader.GetString(0)); //add package name to list
                }
            }
            connection.Close();
            return installedPackages; //returns a list of packages
        }
    }
}