using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using BudgetManager.Models;




namespace BudgetManager.Data;

public static class PackageManager
{
    private const string packageDataBase = "Data Source=PackageDataBase.db";//Address for db

     public static void AddPackageToDataBase(int id, string packageName) //adds package to database
    {
        const string command =
        "INSERT INTO Packages (id, package_name) VALUES ($id, $packageName)";
        using (var connection = new SqliteConnection(packageDataBase))
        {
            connection.Open();
            var addPackageToDataBase = connection.CreateCommand();
            addPackageToDataBase.CommandText = command;
            addPackageToDataBase.Parameters.AddWithValue("$id", id);
            addPackageToDataBase.Parameters.AddWithValue("$packageName", packageName);
            addPackageToDataBase.ExecuteNonQuery();
            connection.Close();
        }
    }



    public static void InstallPackages() //intaller for packages needed
    {
        List<string> installedPackages = GetPackagesFromDb(); //list of installed packages

        foreach (Packages package in GetPackageEnums().OrderBy(p => (int)p))
        {
            string packageName = FormatPackageName(package);

            int id = (int)package; //int value in enum is used as id in package table. It must be converted first

            if (!installedPackages.Contains(packageName)) //if list of table content doesent contain name. It is not installed
            {
                Process.Start("dotnet", $"add package {packageName}")?.WaitForExit();//installs package
                AddPackageToDataBase(id, packageName);//adds package to packagedatabase
            }
            else 
            {
                Console.WriteLine($"Package {packageName} already installed");
            }
        }
    }

    private static List<string> GetPackagesFromDb() //gets all packages from database, these are installed if on in table
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

    private static string FormatPackageName(Packages package) => package.ToString().Replace("_", ".");

    private static List<Packages> GetPackageEnums() => Enum.GetValues(typeof(Packages)).Cast<Packages>().ToList();

}