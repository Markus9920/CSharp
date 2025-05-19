using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;



namespace BudgetManager.Models
{

    public class ExpenseDTO //DTO class to used to return necessary data from API
    {
        public string? Description { get; init; } //only readable
        public double? Cost { get; init; }
        public string? Category { get; init; }

        public ExpenseDTO() //Empty constructor is needed for json deserialization. Otherwise we will get -> "System.NotSupportedException"
        { }

        public ExpenseDTO(string description, double cost, string category)
        {
            Description = description;
            Cost = cost;
            Category = category;
        }
    }
}