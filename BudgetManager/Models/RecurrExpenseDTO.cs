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
    // (userID, Description, cost, recurrenceType, nextOccurrence, category)

    public class RecurrExpenseDTO //DTO class to used to return necessary data from API
    {
        public int ExpenseId { get; private set; }
        public string? Description { get; init; } //init = only readable
        public double? Cost { get; init; }
        public string? RecurrenceType { get; init; }
        public string? Category { get; init; }
        public DateTime OccurrDate { get; init; } //this is the date, when expense must be paid. We calculate the next occurance based on this.
        public DateTime Nextoccurence { get; private set; }
        public bool IsPaid { get; private set; }

        [JsonIgnore]
        public RecurrenceType RecurrenceTypeEnum { get; init; }
        [JsonIgnore]
        public Category CategoryEnum { get; init; }

        public RecurrExpenseDTO() //Empty constructor is needed for json deserialization. Otherwise we will get -> "System.NotSupportedException"
        { }

        public RecurrExpenseDTO(int expenseId, string description, double cost, string recurrenceType, DateTime occurDate, DateTime nextOccurence, bool isPaid, string category)
        {
            ExpenseId = expenseId;
            Description = description;
            Cost = cost;
            RecurrenceType = recurrenceType;
            OccurrDate = occurDate;
            Nextoccurence = nextOccurence;
            IsPaid = isPaid; //we assume that the expense is not paid when it is created, so we make it false
            Category = category;

            //converts strings to enums
            RecurrenceTypeEnum = Enum.Parse<RecurrenceType>(recurrenceType, true);
            CategoryEnum = Enum.Parse<Category>(category, true);
        }
    }
}