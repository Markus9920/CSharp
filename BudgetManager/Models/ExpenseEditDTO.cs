using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using BudgetManager.Models;
using BudgetManager.Data;
using BudgetManager.Services;
using System.Text.Json.Serialization;



namespace BudgetManager.Models
{

    public class ExpenseEditDTO //DTO class to used to return necessary data from API
    {
        public int? ExpenseId { get; init; }
        public string? NewDescription { get; init; }
        public double? NewCostValue { get; init; }

        [JsonIgnore]
        public string? RecurrenceType { get; set; }
        public DateTime? NewNextOccurrDate { get; init; }

        [JsonIgnore]
        public RecurrenceType RecurrenceTypeEnum { get; init; }


        public ExpenseEditDTO() //Empty constructor is needed for json deserialization. Otherwise we will get -> "System.NotSupportedException"
        { }

        public ExpenseEditDTO(int expenseId, string newDescription, double newCostValue, string recurrenceType, DateTime newNextOccurrDate)
        {
            ExpenseId = expenseId;
            NewDescription = newDescription;
            NewCostValue = newCostValue;
            RecurrenceType = recurrenceType;
            NewNextOccurrDate = newNextOccurrDate;

            RecurrenceTypeEnum = Enum.Parse<RecurrenceType>(recurrenceType, true);

        }

    }
} 
