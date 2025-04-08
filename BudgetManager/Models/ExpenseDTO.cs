using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;



namespace BudgetManager.Models;


public class ExpenseDTO //DTO class to used to return necessary data from API
{
    public string? Name { get; set; }
    public double? Cost { get; set; }
    public string? Category { get; set; }

    public ExpenseDTO() //Empty constructor is needed for json deserialization. Otherwise we will get -> "System.NotSupportedException"
    {}

    public ExpenseDTO(string name, double cost, string category)
    {
        Name = name;
        Cost = cost;
        Category = category;
    }
}