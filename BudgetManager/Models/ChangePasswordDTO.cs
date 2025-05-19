using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;



/*this class has all the constructors for handling user data*/

namespace BudgetManager.Models
{
    public class ChangePasswordDTO //DTO class for API to handle data
    {
        public string? Username { get; init; } //only readable
        public string? OldPassword { get; init; }
        public string? NewPassword { get; init; }

        public ChangePasswordDTO() //Empty constructor is needed for json deserialization. Otherwise we will get -> "System.NotSupportedException"
        { }

        public ChangePasswordDTO(int id, string username)
        {
            Username = username;
            OldPassword = OldPassword;
            NewPassword = NewPassword;
        }
    }
}