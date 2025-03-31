using System.Net;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Data.Sqlite;
using ZstdSharp.Unsafe;
using Org.BouncyCastle.Asn1.Misc;
namespace PasswordHash;

public static class ConsoleApp
{
    public static void RunProgram()
    {


        while (true)
        {
            Console.WriteLine("(1) Create account (2) Login (3) Get users from database (4) Get user by id (5) Change password (6) Add mandatory expenses to user \n" +
            "(7) Delete user from database");
            string input = Console.ReadLine() ?? String.Empty;

            if (input == String.Empty)
            {
                Console.WriteLine("Empty input!");
                continue;
            }
            switch (input)
            {
                case "1":
                    CreateAccout();
                    break;
                case "2":
                    Login();
                    break;
                case "3":
                    ShowUsersSavedIntoDataBase();
                    break;
                case "4":
                    ShowUsersSavedIntoDataBase();
                    Console.WriteLine("Give user id");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    {
                        DatabaseManager.GetUserId(id);
                        Console.WriteLine($"User found by id {id}");
                    }
                    else
                    {
                        Console.WriteLine("User not found!");
                    }
                    break;
                case "5":
                    ShowUsersSavedIntoDataBase();
                    UpdatePassword();
                    break;
                case "6":
                    FillMandatoryExpense();
                    break;
                case "7":
                    DeleteUser();
                    break;
                default:
                    Console.WriteLine("Give valid input");
                    break;
            }
        }
    }
    private static int Login()
    {
        string username = "";
        string password = "";
        Console.WriteLine("Log in to user account");
        //Query for username
        while (true)
        {
            Console.Write("Enter username: ");
            username = Console.ReadLine() ?? String.Empty;

            if (username == String.Empty)
            {
                Console.WriteLine("Empty input");
                continue;
            }

            Console.Write("Enter password: ");
            password = Console.ReadLine() ?? String.Empty;

            if (password == String.Empty)
            {
                Console.WriteLine("Empty input");
                continue;
            }

            int? userId = DatabaseManager.Authentication(username, password);
            if (userId != null)
            {

                Console.WriteLine($"Logged in as user id: {userId}");
                return (int)userId;
            }
            Console.WriteLine("Incorrect username or password");
            continue;
        }
    }
    private static void CreateAccout()
    {
        string username = "";
        string password = "";
        //Query for username
        Console.WriteLine("Create user account");
        while (true)
        {
            Console.Write("Give username: ");
            username = Console.ReadLine() ?? String.Empty;

            if (username == String.Empty)
            {
                Console.WriteLine("Give valid username!");
                continue;
            }

            //Query for password
            Console.Write("Give password: ");
            password = Console.ReadLine() ?? String.Empty;

            if (password == String.Empty)
            {
                Console.WriteLine("Give valid password");
                continue;
            }

            //Creates new account
            UserAccount userAccount = new UserAccount(username, password);

            Console.WriteLine($"Hashed password: {userAccount.Password}");
            Console.WriteLine($"Salt: {userAccount.Salt}");

            if (DatabaseManager.AddUserToDataBase(userAccount.Username, userAccount.Password, userAccount.Salt))
            {
                Console.WriteLine($"User {userAccount.Username} created succesfully!");
                break;
            }
            else
            {
                Console.WriteLine("User already exists!");
                continue;
            }
        }
    }
    private static void DeleteUser()
    {
        //Method to remove user from database.
        DatabaseManager.GetAllUsers(); //gets all the users from database
        while (true)
        {
            Console.WriteLine("Give ID for deleting user");

            if (int.TryParse(Console.ReadLine(), out int userId)) //if id is empty, or it cannot be parsed to int
            {
                if (DatabaseManager.DeleteUserById(userId))
                {
                    Console.WriteLine($"User {userId} deleted succesfully");
                }
                else
                {
                    Console.WriteLine("User not found!");
                }
            }
            else
            {
                Console.WriteLine("Invalid Input!");
            }
        }
    }
    private static void ShowUsersSavedIntoDataBase()
    {
        var users = DatabaseManager.GetAllUsers();
        foreach (var user in users)
        {
            Console.WriteLine($"{user.Id} {user.Username}");
        }
    }

    private static void UpdatePassword()
    {
        string username = "";
        string password = "";
        string newPassword = "";

        while (true)
        {
            Console.Write("Enter username: ");
            username = Console.ReadLine() ?? String.Empty;

            if (username == String.Empty)
            {
                Console.WriteLine("Empty input");
                continue;
            }

            Console.Write("Enter password: ");
            password = Console.ReadLine() ?? String.Empty;

            if (password == String.Empty)
            {
                Console.WriteLine("Empty input");
                continue;
            }

            Console.Write("Enter new password: ");
            newPassword = Console.ReadLine() ?? String.Empty;

            if (newPassword == String.Empty)
            {
                Console.WriteLine("Empty input");
                continue;
            }

            if (DatabaseManager.ChangePassword(username, password, newPassword))
            {
                Console.WriteLine($"{username} password {password} changed to {newPassword}");
                break;
            }
            else
            {
                Console.WriteLine("Error occured, try again");
                break;
            }

        }
    }
    private static void FillMandatoryExpense()
    {

        string expenseName = "";
        double costAmount = 0;
        int userId = Login();

        while (true)
        {   
            Console.WriteLine("Press (x) to leave");
            Console.Write("Give name for expense to add: ");
            expenseName = Console.ReadLine() ?? string.Empty;

            if (expenseName.ToLower().Trim() == "x")
            {
                Console.WriteLine($"Total mandatory expense cost: {DatabaseManager.SumMandatoryExpenses(userId).ToString()}");
                break;
            }

            if (expenseName == String.Empty)
            {
                Console.WriteLine("Give input!");
                continue;
            }

            Console.WriteLine();
            Console.Write("How much is the cost for expense?: ");
            string cost = Console.ReadLine() ?? string.Empty;

            if (!double.TryParse(cost, out costAmount))
            {
                Console.WriteLine("Invalid input!");
                continue;
            }

            DatabaseManager.AddExpense(userId, expenseName, costAmount);
            Console.WriteLine($"********Added to {userId} expense: {expenseName} with cost of {costAmount}********");
        }
    }
}