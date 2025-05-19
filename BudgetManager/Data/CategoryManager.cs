using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using BudgetManager.Models;


namespace BudgetManager.Data

{

    public static class CategoryManager
    {
        private static readonly string dataBase = "Data Source=Database.db";


        private static void AddCategoryToDatabase(int id, string name)
        {
            string command = "INSERT INTO Categories (id, name) VALUES ($id, $name)";
            try
            {
                using (SqliteConnection connection = new SqliteConnection(dataBase))
                {
                    var addCategoryToDatabaseCommand = connection.CreateCommand();
                    addCategoryToDatabaseCommand.CommandText = command;
                    addCategoryToDatabaseCommand.Parameters.AddWithValue("$id", id);
                    addCategoryToDatabaseCommand.Parameters.AddWithValue("$name", name);

                    connection.Open();
                    addCategoryToDatabaseCommand.ExecuteNonQuery();
                    connection.Close();

                }
            }
            catch (SqliteException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //returns a list of enums
        private static List<Category> GetCategoryEnums() => Enum.GetValues(typeof(Category)).Cast<Category>().ToList();

        public static string FormatCategoryName(Category category) => category.ToString().Replace("_", " ");

        public static void FillCategoriesTable()
        {
            var categoryIdsFromDb = GetCategoryIdsFromDb(); //gets categories from db

            foreach (Category category in GetCategoryEnums())
            {
                int id = (int)category;//converts the variable stored into enum as integer, so it can be used to make it as id int sql table

                if (!categoryIdsFromDb.Contains(id)) // if Categories table does not contain id
                {
                    string name = FormatCategoryName(category); //category enum name to string

                    AddCategoryToDatabase(id, name);
                }
            }
        }

        private static List<int> GetCategoryIdsFromDb() //get all categories stored into db
        {
            string command = "SELECT id FROM Categories";

            List<int> idList = new List<int>();

            using (var connection = new SqliteConnection(dataBase))
            {
                var getCategoriesFromDbCommand = connection.CreateCommand();
                getCategoriesFromDbCommand.CommandText = command;

                connection.Open();
                var result = getCategoriesFromDbCommand.ExecuteReader();

                while (result.Read())
                {
                    idList.Add(result.GetInt32(0));


                }
                connection.Close();
            }
            return idList;
        }
    }
}