using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using BudgetManager.Models;


namespace BudgetManager.Data;



public static class CategoryManager
{
    private static readonly string dataBase = "Data Source=Database.db";


    private static void AddCategoryToDatabase(int id, string name)
    {
        string command = "INSERT INTO Categories (id, name) VALUES ($id, $name)";
        try
        {
            using (var connection = new SqliteConnection(dataBase))
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

    private static List<Categories> GetCategoryEnums() //gets enums and adds to list. Enums are categories.
    {
        return Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
    }

    public static void FillCategoriesTable()
    {
        var categoryIdsFromDb = GetCategoryIdsFromDb(); //gets categories from db

        foreach (var category in GetCategoryEnums())
        {
            int id = Convert.ToInt32(category);//converts the variable stored into enum as integer, so it can be used to make it as id int sql table

            if (!categoryIdsFromDb.Contains(id)) // if Categories table does not contan id
            {
                string name = category.ToString(); //category enum name to string

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


    //*************for console use**************
    public static void ShowCategories()
    {

        string command = "SELECT id, name FROM Categories";

        using (var connection = new SqliteConnection(dataBase))
        {
            var getCategoriesFromDbCommand = connection.CreateCommand();
            getCategoriesFromDbCommand.CommandText = command;

            connection.Open();
            var result = getCategoriesFromDbCommand.ExecuteReader();

            while (result.Read())
            {
                Console.WriteLine($"id: {result.GetInt32(0)} {result.GetString(1)}");
            }
            connection.Close();
        }
    }
}
