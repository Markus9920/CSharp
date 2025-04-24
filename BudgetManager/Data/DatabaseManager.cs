using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using BudgetManager.Models;
using System.Globalization;


namespace BudgetManager.Data
{
    public static class DatabaseManager
    {
        private const int saltSize = 16;//Used for salting and hashing new password
        private const int iterations = 350000;//Used for salting and hashing new password
        private const int keySize = 64;//Used for salting and hashing new password

        public static readonly string recurringExpenseTableName = "RecurringExpenses";
        public static readonly string otherExpensesTableName = "OtherExpenses";

        private static readonly string dataBase = "Data Source=dataBase.db";//Address for db


        #region useraccount database management

        #endregion
        #region methods for account management in database
        //**** Methods for users table******
        public static bool AddUserToDataBase(string username, string password, string salt) // add new user to database, if not existing
        {
            string command =
            "INSERT INTO Users (username, passwordhash, salt) VALUES ($username, $passwordhash, $salt)";

            try
            {
                using (var connection = new SqliteConnection(dataBase))
                {

                    var addUserCommand = connection.CreateCommand();
                    addUserCommand.CommandText = command;
                    addUserCommand.Parameters.AddWithValue("$username", username);
                    addUserCommand.Parameters.AddWithValue("$passwordhash", password);
                    addUserCommand.Parameters.AddWithValue("$salt", salt);

                    connection.Open();
                    int row = addUserCommand.ExecuteNonQuery();
                    connection.Close();

                    return row > 0; //affected row greater than zero means its true
                }
            }
            catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067) // UNIQUE constraint error
            {
                return false; //false, user already exists
            }
        }

        public static int Authentication(string username, string passwordInput) //loggin in, checks password and username
        {
            string command =
            "SELECT id, passwordhash, salt FROM Users WHERE username = $username";

            using (var connection = new SqliteConnection(dataBase))
            {

                var checkPasswordCommand = connection.CreateCommand();
                checkPasswordCommand.CommandText = command;
                checkPasswordCommand.Parameters.AddWithValue("$username", username);

                connection.Open();
                using (var result = checkPasswordCommand.ExecuteReader())
                {
                    if (result.Read())
                    {
                        int userId = result.GetInt32(0);
                        string passwordHashFromDataBase = result.GetString(1);
                        string saltFromDataBase = result.GetString(2);


                        if (UserAccount.Verify(passwordInput, passwordHashFromDataBase, saltFromDataBase))
                        {
                            return userId; //return userid if found
                        }
                    }
                }
                connection.Close();
                return -1; //return -1 if nothing found
            }
        }
        public static bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            string command = "SELECT passwordhash, salt FROM Users WHERE id = $id";

            using (var connection = new SqliteConnection(dataBase))
            {
                string passwordHashFromDataBase = "";
                string saltFromDataBase = "";

                var getUserCommand = connection.CreateCommand();
                getUserCommand.CommandText = command;
                getUserCommand.Parameters.AddWithValue("$id", userId);

                connection.Open();

                using (var result = getUserCommand.ExecuteReader())
                {
                    if (result.Read())
                    {
                        passwordHashFromDataBase = result.GetString(0);
                        saltFromDataBase = result.GetString(1);
                    }
                    else
                    {
                        return false; // user not found
                    }
                }

                //verify the old password
                bool isCorrect = UserAccount.Verify(oldPassword, passwordHashFromDataBase, saltFromDataBase);

                Console.WriteLine(oldPassword);
                Console.WriteLine(passwordHashFromDataBase);
                Console.WriteLine(saltFromDataBase);
                if (!isCorrect)
                {
                    return false;
                }

                //Creates new password
                var (hash, salt) = UserAccount.NewPassword(newPassword);
                string newHash = Convert.ToHexString(hash);
                string newSalt = Convert.ToHexString(salt);

                string updateCommandText = "UPDATE Users SET passwordhash = $passwordhash, salt = $salt WHERE id = $id";

                var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = updateCommandText;
                updateCommand.Parameters.AddWithValue("$passwordhash", newHash);
                updateCommand.Parameters.AddWithValue("$salt", newSalt);
                updateCommand.Parameters.AddWithValue("$id", userId);

                int rows = updateCommand.ExecuteNonQuery();
                connection.Close();

                return rows > 0;
            }
        }


        #region Admin methods(admin account not existing yet)
        public static List<UserDTO> GetAllUsers() //get all users from bd and put into list of Data Transfer Objects
        {
            string command = "SELECT id, username FROM Users ORDER BY id";

            List<UserDTO> users = new List<UserDTO>();

            using (var connection = new SqliteConnection(dataBase))
            {

                var getAllUsersCommand = connection.CreateCommand();
                getAllUsersCommand.CommandText = command;

                connection.Open();
                using (var result = getAllUsersCommand.ExecuteReader())
                {
                    while (result.Read())
                    {
                        users.Add(new UserDTO(result.GetInt32(0), result.GetString(1))); //add to list
                    }
                }

                connection.Close();
                return users; //return list, empty if nothing is found
            }
        }

        //this may not be needed, Authentication already returns user id.
        public static UserDTO? GetUserId(int id) //finds user by id number
        {
            string command = "SELECT id, username FROM Users WHERE id = $id";

            using (var connection = new SqliteConnection(dataBase))
            {
                connection.Open();
                var getUserIdCommand = connection.CreateCommand();
                getUserIdCommand.CommandText = command;
                getUserIdCommand.Parameters.AddWithValue("$id", id);

                using (var result = getUserIdCommand.ExecuteReader())
                {
                    if (result.Read())
                    {
                        return new UserDTO(result.GetInt32(0), result.GetString(1)); //returns new Data Transfer Object
                    }
                }

                connection.Close();
                return null; //otherwise returns null
            }
        }

        public static bool DeleteUserById(int id) //delete user by id
        {
            string command = "DELETE FROM Users WHERE id = $id";

            using (var connection = new SqliteConnection(dataBase))
            {

                var deleteUserCommand = connection.CreateCommand();
                deleteUserCommand.CommandText = command;
                deleteUserCommand.Parameters.AddWithValue("$id", id);

                connection.Open();
                int row = deleteUserCommand.ExecuteNonQuery();
                connection.Close();

                return row > 0; //affected row greater than zero means its true
            }
        }
        #endregion


        #endregion
        #region These methods work for both expense tables

        public static double SumExpenses(int userId, string tableName)
        {
            if (tableName != recurringExpenseTableName && tableName != otherExpensesTableName)
            {
                throw new ArgumentException("Invalid table name", nameof(tableName));
            }

            string command =
            $"SELECT SUM(cost) FROM {tableName} WHERE userID = $userId";
            double totalExpenseCost = 0;

            using (var connection = new SqliteConnection(dataBase))
            {
                var sumMandatoryExpensesCommand = connection.CreateCommand();
                sumMandatoryExpensesCommand.CommandText = command;
                sumMandatoryExpensesCommand.Parameters.AddWithValue("$userId", userId);

                connection.Open();
                var result = sumMandatoryExpensesCommand.ExecuteScalar();
                connection.Close();
                // If the result is not null, convert it to double; otherwise, use 0 as the default value
                return totalExpenseCost = result != null ? Convert.ToDouble(result) : 0;
            }
        }
        //Method for removing expense - Метод для удаления обязательного расхода
        public static bool DeleteExpense(int expenseId, int userId, string tableName)
        {
            if (tableName != recurringExpenseTableName && tableName != otherExpensesTableName)
            {
                throw new ArgumentException("Invalid table name", nameof(tableName));
            }

            using (var connection = new SqliteConnection(dataBase))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"DELETE FROM {tableName} WHERE expenseID = $expenseID AND userID = $userID";
                command.Parameters.AddWithValue("$expenseID", expenseId);
                command.Parameters.AddWithValue("$userID", userId);
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();
                return rowsAffected > 0;
            }

            //Copyright
            //*** This code was written by Olesia -edited by Markus***
        }

        public static DataTable GetExpensesForUser(int userId, string tableName)
        {
            if (tableName != recurringExpenseTableName && tableName != otherExpensesTableName)
            {
                throw new ArgumentException("Invalid table name", nameof(tableName));
            }


            DataTable dt = new DataTable();
            using (var connection = new SqliteConnection(dataBase))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT expenseID, name, cost FROM {tableName} WHERE userID = $userID";
                command.Parameters.AddWithValue("$userID", userId);
                using (var reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                }
                connection.Close();
            }
            return dt;

            //Copyright
            //*** This code was written by Olesia -edited by Markus***
        }

        #endregion
        #region Methods for recurring expenses table

        //Method for updating expense record - Метод для обновления записи обязательного расхода
        public static bool UpdateRecurringExpense(int expenseId, int userId, string name, double cost) //lisää tähän vielä päivämäärä -muuttujat
        {
            using (var connection = new SqliteConnection(dataBase))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE OtherExpenses 
                    SET name = $name, cost = $cost 
                    WHERE expenseID = $expenseID AND userID = $userID
                ";
                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$cost", cost);
                command.Parameters.AddWithValue("$expenseID", expenseId);
                command.Parameters.AddWithValue("$userID", userId);
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();
                return rowsAffected > 0;
            }

            //Copyright
            //*** This code was written by Olesia -edited by Markus***
        }

        // Метод для добавления нового периодического расхода
        // recurrenceType может быть "Monthly" или "Yearly"
        public static bool AddRecurringExpense(int userId, string name, double cost, string recurrenceType, DateTime startDate, string category)
        {

            using (var connection = new SqliteConnection(dataBase))
            {
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO RecurringExpenses (userID, name, cost, recurrenceType, nextOccurrence, category) VALUES ($userID, $name, $cost, $recurrenceType, $nextOccurrence, $category)";
                command.Parameters.AddWithValue("$userID", userId);
                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$cost", cost);
                command.Parameters.AddWithValue("$recurrenceType", recurrenceType); // e.g. "Monthly" or "Yearly"
                command.Parameters.AddWithValue("$nextOccurrence", startDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("$category", category);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();
                return rowsAffected > 0;

            }
        }

        #endregion

        #region Methods for other expenses table

        public static bool AddOtherExpense(int userId, string description, double amount)
        {
            using (var connection = new SqliteConnection(dataBase))
            {
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO OtherExpenses (userID, description, amount) VALUES ($userID, $description, $amount)";
                command.Parameters.AddWithValue("$userID", userId);
                command.Parameters.AddWithValue("$description", description);
                command.Parameters.AddWithValue("$amount", amount);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                return rowsAffected > 0;
            }
            //Copyright
            //*** This code was written by Konsta
        }

        public static DataTable GetOtherExpensesForUser(int userId)
        {
            DataTable dt = new DataTable();
            using (var connection = new SqliteConnection(dataBase))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT expenseID, description, amount FROM OtherExpenses WHERE userID = $userID";
                command.Parameters.AddWithValue("$userID", userId);
                using (var reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                }
                connection.Close();
            }
            return dt;
            //Copyright
            //*** This code was written by Konsta
        }

        public static bool UpdateOtherExpense(int expenseId, int userId, string description, double amount)
        {
            using (var connection = new SqliteConnection(dataBase))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE OtherExpenses 
                    SET description = $description, amount = $amount 
                    WHERE expenseID = $expenseID AND userID = $userID
                ";
                command.Parameters.AddWithValue("$description", description);
                command.Parameters.AddWithValue("$amount", amount);
                command.Parameters.AddWithValue("$expenseID", expenseId);
                command.Parameters.AddWithValue("$userID", userId);
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();
                return rowsAffected > 0;
            }
            //Copyright
            //*** This code was written by Konsta
        }

        public static bool DeleteOtherExpense(int expenseId, int userId)
        {
            using (var connection = new SqliteConnection(dataBase))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM OtherExpenses WHERE expenseID = $expenseID AND userID = $userID";
                command.Parameters.AddWithValue("$expenseID", expenseId);
                command.Parameters.AddWithValue("$userID", userId);
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();
                return rowsAffected > 0;
            }
            //Copyright
            //*** This code was written by Konsta
        }

        public static double SumOtherExpenses(int userId)
        {
            string command = "SELECT SUM(amount) FROM OtherExpenses WHERE userID = $userId";
            double totalExpenseCost = 0;

            using (var connection = new SqliteConnection(dataBase))
            {
                var sumOtherExpensesCommand = connection.CreateCommand();
                sumOtherExpensesCommand.CommandText = command;
                sumOtherExpensesCommand.Parameters.AddWithValue("$userId", userId);

                connection.Open();
                var result = sumOtherExpensesCommand.ExecuteScalar();
                connection.Close();
                // If the result is not null, convert it to double; otherwise, use 0 as the default value
                return totalExpenseCost = result != null ? Convert.ToDouble(result) : 0;
            }
            //Copyright
            //*** This code was written by Konsta
        }
    }
}
#endregion