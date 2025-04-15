using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;



/*this class has all the constructors for handling user data*/

namespace BudgetManager.Models;

public class NewUserDTO //DTO class for API to handle data
{

    public string? Username { get; init; } //only readable
    public string? Password { get; init; }


    public NewUserDTO() //Empty constructor is needed for json deserialization. Otherwise we will get -> "System.NotSupportedException"
    { }

    public NewUserDTO(string username, string password)
    {
        Password = password;
        Username = username;
    }
}
