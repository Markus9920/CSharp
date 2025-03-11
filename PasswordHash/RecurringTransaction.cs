using System.Net;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Data.Sqlite;
namespace PasswordHash;

public class RecurringTransaction
{
    public string Name {get; private set;}

    public double Cost {get; private set;}


    public RecurringTransaction(string name, double cost)
    {
        Name = name;
        Cost = cost;
    }

    
}