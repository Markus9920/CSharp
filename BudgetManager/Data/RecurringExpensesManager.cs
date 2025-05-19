using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using BudgetManager.Models;
using BudgetManager.Services;

namespace BudgetManager.Data
{
    //Copyright
    //*** This code was written by Olesia -edited by Markus -modified the code, so it returns
    //a list of expenses if it happens on the current month. Also added category for expense
    //also DateTime newNextOccurrence = RecurExpenseService.SetCorrectNextOccuranceType(nextOccurrence, recurrenceType);
    //and recurrExpenseDTOlist.Add(new RecurrExpenseDTO(name, cost, recurrenceType.ToString(), occurenceDate, nextOccurrence, category.ToString()));

    public static class RecurringExpensesManager
    {
        private static readonly string dataBase = "Data Source=Database.db"; // Используем ту же строку подключения
                                                                             //use the same connection string as in DatabaseCreator.cs


        // Метод для обработки периодических расходов и их автоматического добавления, если наступила дата
        // следующего расхода
        //This method checks the next occurrence date of each recurring expense in the database.
        // If the date is today or earlier, it adds the expense to the mandatory expenses table and updates the next occurrence date.
        public static bool ProcessRecurringExpenses(int userId)
        {
            bool rowsUpdated = false;
            using (var connection = new SqliteConnection(dataBase))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT expenseID, recurrenceType, nextOccurrence FROM RecurringExpenses WHERE userID = $userId";
                command.Parameters.AddWithValue("$userId", userId);
                using (var reader = command.ExecuteReader())
                {

                    if (!reader.HasRows)
                    {
                        return false;
                    }



                    while (reader.Read())
                    {

                        if (reader.IsDBNull(0) || reader.IsDBNull(1) || reader.IsDBNull(2))
                        {
                            continue;
                        }


                        int expenseId = reader.GetInt32(0);
                        int recurrenceTypeInt = reader.GetInt32(1); //Enum comes as int from the database(look from the RecurrenceType.cs file)
                        string nextOccurrenceStr = reader.GetString(2);


                        //change date into correct type
                        DateTime nextOccurrence = DateTime.Parse(nextOccurrenceStr);
                        //change data from db back to enum type
                        RecurrenceType recurrenceType = (RecurrenceType)recurrenceTypeInt;

                        // Проверяем, наступила ли дата следующего расхода (сегодня или ранее)
                        // Check if the next occurrence date is today or earlier
                        if (nextOccurrence.Year == DateTime.Now.Year && nextOccurrence.Month == DateTime.Now.Month)
                        {


                            // Вычисляем новую дату следующего расхода в зависимости от типа повторения
                            // Calculate the new next occurrence date based on the recurrence type
                            DateTime newNextOccurrence = RecurExpenseService.SetCorrectNextOccuranceType(nextOccurrence, recurrenceType);


                            // Обновляем дату следующего наступления в таблице периодических расходов
                            // Update the next occurrence date in the recurring expenses table
                            var updateCommand = connection.CreateCommand();
                            updateCommand.CommandText = "UPDATE RecurringExpenses SET nextOccurrence = $nextOccurrence, isPaid = 0 WHERE expenseID = $expenseID";
                            updateCommand.Parameters.AddWithValue("$nextOccurrence", newNextOccurrence.ToString("yyyy-MM-dd"));
                            updateCommand.Parameters.AddWithValue("$expenseID", expenseId);
                            updateCommand.ExecuteNonQuery();

                            rowsUpdated = true;
                        }
                    }
                }
            }
            return rowsUpdated;
        }

        public static bool MarkRecurrExpensePaid(int expenseId, int userId)
        {
            string command = @"UPDATE RecurringExpenses SET isPaid = 1 WHERE expenseID = $expenseId and userID = $userId";

            using (var connection = new SqliteConnection(dataBase))
            {
                var setPaidCommand = connection.CreateCommand();
                setPaidCommand.CommandText = command;
                setPaidCommand.Parameters.AddWithValue("$expenseId", expenseId);
                setPaidCommand.Parameters.AddWithValue("userId", userId);

                connection.Open();
                int rowsAffected = setPaidCommand.ExecuteNonQuery();
                connection.Close();

                return rowsAffected > 0;
            }
        }
        public static bool MarkRecurrExpenseNotPaid(int expenseId, int userId)
        {
            string command = @"UPDATE RecurringExpenses SET isPaid = 0 WHERE expenseID = $expenseId and userID = $userId";

            using (var connection = new SqliteConnection(dataBase))
            {
                var setPaidCommand = connection.CreateCommand();
                setPaidCommand.CommandText = command;
                setPaidCommand.Parameters.AddWithValue("$expenseId", expenseId);
                setPaidCommand.Parameters.AddWithValue("userId", userId);

                connection.Open();
                int rowsAffected = setPaidCommand.ExecuteNonQuery();
                connection.Close();

                return rowsAffected > 0;
            }
        }

        //Method for updating expense record - Метод для обновления записи обязательного расхода
        public static bool UpdateRecurringExpense(int expenseId, int userId, string newDescriptionValue, double newCostValue, DateTime newNextOccurenceValue) //lisää tähän vielä päivämäärä -muuttujat
        {
            string selectCommand = @"SELECT description, cost, nextOccurrence, recurrenceType FROM RecurringExpenses
            WHERE expenseID = $expenseId AND userID = $userId";



            //first we have to get the current values from database
            using (var connection = new SqliteConnection(dataBase))
            {
                connection.Open();
                //Later we save the values from db into variables below. 
                //these are used later for comparing
                string descriptionFromDb = "";
                double costFromDb = 0.0;
                DateTime nextOccurrFromBd = DateTime.MinValue; // 01.01.0001 00:00:00

                var selectValuesCommand = connection.CreateCommand();
                selectValuesCommand.CommandText = selectCommand;
                selectValuesCommand.Parameters.AddWithValue("$expenseId", expenseId);
                selectValuesCommand.Parameters.AddWithValue("$userId", userId);


                using (var result = selectValuesCommand.ExecuteReader())
                {
                    while (result.Read())
                    {
                        descriptionFromDb = result.GetString(0);
                        costFromDb = result.GetDouble(1);
                        nextOccurrFromBd = DateTime.Parse(result.GetString(2));
                    }
                }

                //now we have the current data we need from database
                //next we compare the new values from method parameters to current values and check if they are different

                string updateCommand = @"
                    UPDATE RecurringExpenses 
                    SET description = $newDescriptionValue, cost = $newCostValue, nextOccurrence = $newNextOccurenceValue  
                    WHERE expenseID = $expenseId AND userID = $userId";

                //new values to store in database, if edited by the user
                //keeps the same value from bd, if not changed by the user
                string? updatedDescription = descriptionFromDb;
                double? updatedCost = costFromDb;
                DateTime updatedNextOccurr = nextOccurrFromBd;

                if (!string.IsNullOrWhiteSpace(newDescriptionValue) && newDescriptionValue != descriptionFromDb)
                {
                    updatedDescription = newDescriptionValue;//value is changed to user given value
                }
                if (newCostValue != costFromDb)
                {
                    updatedCost = newCostValue;
                }
                if (updatedNextOccurr != newNextOccurenceValue)
                {
                    updatedNextOccurr = newNextOccurenceValue;
                }
                //so, now the value is only vhanged if there is really different value in method parameter

                var updateExpenseCommand = connection.CreateCommand();
                updateExpenseCommand.CommandText = updateCommand;
                updateExpenseCommand.Parameters.AddWithValue("$expenseId", expenseId);
                updateExpenseCommand.Parameters.AddWithValue("$userId", userId);
                updateExpenseCommand.Parameters.AddWithValue("$newDescriptionValue", updatedDescription);
                updateExpenseCommand.Parameters.AddWithValue("$newCostValue", updatedCost);
                updateExpenseCommand.Parameters.AddWithValue("$newNextOccurenceValue", updatedNextOccurr);

                int rowsAffected = updateExpenseCommand.ExecuteNonQuery();
                connection.Close();

                return rowsAffected > 0;
            }

            //Copyright
            //*** This code was written by Olesia -edited by Markus***
        }

        // Метод для добавления нового периодического расхода
        // recurrenceType может быть "Monthly" или "Yearly"
        public static bool AddRecurringExpense(int userId, string description, double cost, RecurrenceType recurrenceType, DateTime occurenceDate, DateTime nextOccurrence, bool isPaid, Category category)
        {
            using (var connection = new SqliteConnection(dataBase))
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
            INSERT INTO RecurringExpenses (userID, description, cost, occurenceDate, recurrenceType, nextOccurrence, category)
            VALUES ($userID, $description, $cost, $occurenceDate, $recurrenceType, $nextOccurrence, $category)";

                command.Parameters.AddWithValue("$userID", userId);
                command.Parameters.AddWithValue("$description", description);
                command.Parameters.AddWithValue("$cost", cost);
                command.Parameters.AddWithValue("$occurenceDate", occurenceDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("$recurrenceType", recurrenceType);
                command.Parameters.AddWithValue("$nextOccurrence", nextOccurrence.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("$isPaid", isPaid);
                command.Parameters.AddWithValue("$category", category);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();
                return rowsAffected > 0;
            }
        }
        public static List<RecurrExpenseDTO> GetUnpaidRecurrExpensesFromDb(int userId)
        {
            List<RecurrExpenseDTO> unPaidExpenses = new List<RecurrExpenseDTO>();

            using (var connection = new SqliteConnection(dataBase))
            {
                string command = @"
            SELECT expenseID, description, cost, occurenceDate, recurrenceType, nextOccurrence, isPaid, category 
            FROM RecurringExpenses WHERE $userID = userID AND isPaid = 0";

                var getUnpaidExpenses = connection.CreateCommand();
                getUnpaidExpenses.CommandText = command;
                getUnpaidExpenses.Parameters.AddWithValue("$userID", userId);

                connection.Open();
                using (var result = getUnpaidExpenses.ExecuteReader())
                {
                    while (result.Read())
                    {
                        int expenseId = result.GetInt32(0);
                        string description = result.GetString(1);
                        double cost = result.GetDouble(2);
                        DateTime occurence = DateTime.Parse(result.GetString(3));
                        int recurrenceTypeInt = result.GetInt32(4);
                        DateTime nextOccurence = DateTime.Parse(result.GetString(5));
                        bool isPaid = result.GetBoolean(6);
                        int categoryTypeInt = result.GetInt32(7);

                        string recurrenceType = ((RecurrenceType)recurrenceTypeInt).ToString();
                        string category = ((Category)categoryTypeInt).ToString();

                        unPaidExpenses.Add(new RecurrExpenseDTO(expenseId, description, cost, recurrenceType, occurence, nextOccurence, isPaid, category));
                    }
                }
                connection.Close();
                return unPaidExpenses;
            }
        }
        public static List<RecurrExpenseDTO> GetAllRecurrExpensesFromDb(int userId)
        {
            List<RecurrExpenseDTO> allRecurrExpenses = new List<RecurrExpenseDTO>();

            using (var connection = new SqliteConnection(dataBase))
            {
                string command = @"
            SELECT expenseID, description, cost, occurenceDate, recurrenceType, nextOccurrence, isPaid, category 
            FROM RecurringExpenses WHERE $userID = userID";

                var getUnpaidExpenses = connection.CreateCommand();
                getUnpaidExpenses.CommandText = command;
                getUnpaidExpenses.Parameters.AddWithValue("$userID", userId);

                connection.Open();
                using (var result = getUnpaidExpenses.ExecuteReader())
                {
                    while (result.Read())
                    {
                        int expenseId = result.GetInt32(0);
                        string description = result.GetString(1);
                        double cost = result.GetDouble(2);
                        DateTime occurence = DateTime.Parse(result.GetString(3));
                        int recurrenceTypeInt = result.GetInt32(4);
                        DateTime nextOccurence = DateTime.Parse(result.GetString(5));
                        bool isPaid = result.GetBoolean(6);
                        int categoryTypeInt = result.GetInt32(7);

                        string recurrenceType = ((RecurrenceType)recurrenceTypeInt).ToString();
                        string category = ((Category)categoryTypeInt).ToString();

                        allRecurrExpenses.Add(new RecurrExpenseDTO(expenseId, description, cost, recurrenceType, occurence, nextOccurence, isPaid, category));
                    }
                }
                connection.Close();
                return allRecurrExpenses;
            }
        }
    }
}