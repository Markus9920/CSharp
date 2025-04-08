using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using BudgetManager.Models;

namespace BudgetManager.Data;

        //Copyright
        //*** This code was written by Olesia -edited by Markus -modified the code, so it returns
        //a list of expenses if it happens on the current month. Also added category for expense

public static class RecurringExpensesManager
{
    private static readonly string dataBase = "Data Source=Database.db"; // Используем ту же строку подключения
    //use the same connection string as in DatabaseCreator.cs

   
    // Метод для обработки периодических расходов и их автоматического добавления, если наступила дата
    // следующего расхода
    //This method checks the next occurrence date of each recurring expense in the database.
    // If the date is today or earlier, it adds the expense to the mandatory expenses table and updates the next occurrence date.
    public static List<ExpenseDTO> ProcessRecurringExpenses()
    {
        List<ExpenseDTO> expenseDTOlist = new List<ExpenseDTO>();

        using (var connection = new SqliteConnection(dataBase))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT expenseID, userID, name, cost, recurrenceType, nextOccurrence, category FROM RecurringExpenses";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int expenseId = reader.GetInt32(0);
                    int userId = reader.GetInt32(1);
                    string name = reader.GetString(2);
                    double cost = reader.GetDouble(3);
                    string recurrenceType = reader.GetString(4);
                    string nextOccurrenceStr = reader.GetString(5);
                    DateTime nextOccurrence = DateTime.Parse(nextOccurrenceStr);
                    string category = reader.GetString(6);
                    // Проверяем, наступила ли дата следующего расхода (сегодня или ранее)
                    // Check if the next occurrence date is today or earlier
                    if (nextOccurrence.Year == DateTime.Now.Year && nextOccurrence.Month == DateTime.Now.Month)
                    {
                        // Добавляем расход в таблицу обязательных расходов
                        // Add the expense to the mandatory expenses table
                        // DatabaseManager.AddExpense(userId, name, cost);

                        expenseDTOlist.Add(new ExpenseDTO(name, cost, category));

                        // Вычисляем новую дату следующего расхода в зависимости от типа повторения
                        // Calculate the new next occurrence date based on the recurrence type
                        DateTime newNextOccurrence;
                        if (recurrenceType.Equals("Monthly", StringComparison.OrdinalIgnoreCase))
                        {
                            newNextOccurrence = nextOccurrence.AddMonths(1);
                        }
                        else if (recurrenceType.Equals("Yearly", StringComparison.OrdinalIgnoreCase))
                        {
                            newNextOccurrence = nextOccurrence.AddYears(1);
                        }
                        else
                        {
                            // Если тип не распознан, оставляем прежнюю дату (или можно реализовать другую логику)
                            // If the type is not recognized, keep the old date (or implement other logic)
                            newNextOccurrence = nextOccurrence;
                        }

                        // Обновляем дату следующего наступления в таблице периодических расходов
                        // Update the next occurrence date in the recurring expenses table
                        var updateCommand = connection.CreateCommand();
                        updateCommand.CommandText = "UPDATE RecurringExpenses SET nextOccurrence = $nextOccurrence WHERE expenseID = $expenseID";
                        updateCommand.Parameters.AddWithValue("$nextOccurrence", newNextOccurrence.ToString("yyyy-MM-dd"));
                        updateCommand.Parameters.AddWithValue("$expenseID", expenseId);
                        updateCommand.ExecuteNonQuery();
                    }
                }
                connection.Close();
                return expenseDTOlist;
            }
        }
    }
}