using System.Security.Claims;
using BudgetManager.Services;
using BudgetManager.Data;
using BudgetManager.Models;
using Microsoft.AspNetCore.Mvc;


namespace BudgetManager.Services
{
    public static class RecurExpenseService
    {


        public static DateTime SetCorrectNextOccuranceType(DateTime current, RecurrenceType recurrType)
        {
            switch (recurrType)
            {
                case RecurrenceType.Yearly:
                    return current.AddYears(1);
                case RecurrenceType.Monthly:
                    return current.AddMonths(1);
                default:
                    return current;    
            }
        }
    }
}