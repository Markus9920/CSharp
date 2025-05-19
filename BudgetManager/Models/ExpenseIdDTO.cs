using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;



namespace BudgetManager.Models
{

    public class ExpenseIdDTO //DTO class to used to return necessary data from API
    {
        public int ExpenseId { get; init; }

        

        public ExpenseIdDTO() //Empty constructor is needed for json deserialization. Otherwise we will get -> "System.NotSupportedException"
        { }

        public ExpenseIdDTO(int expenseId)
        {
            ExpenseId = expenseId;
        }
        
    }
}