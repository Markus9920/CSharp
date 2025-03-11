using System.Net;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public static class PackageManager
{
    public static void InstallSQLPackages()
    {
        string sqlite = "Microsoft.Data.Sqlite";
        string mysql = "MySql.Data";
        string postgreSQL = "Npgsql";

        Console.WriteLine("**********Installing SQL packages needed for this program. This may take several minutes**********");

        Process.Start("dotnet", $"add package {sqlite}")?.WaitForExit();
        Process.Start("dotnet", $"add package {mysql}")?.WaitForExit();
        Process.Start("dotnet", $"add package {postgreSQL}")?.WaitForExit();

        Console.WriteLine("\n**********Package installation complete*********\n");
    }
}