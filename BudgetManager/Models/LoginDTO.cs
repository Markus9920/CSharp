using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;



namespace BudgetManager.Models;


public class LoginDTO //DTO class for API to handle data
{
    public string? Username { get; set; }
    public string? Password { get; set; }

    public LoginDTO() //Empty constructor is needed for json deserialization. Otherwise we will get -> "System.NotSupportedException"
    { }
    
    public LoginDTO(string username, string password) 
    {
        Password = password;
        Username = username;
    }
}