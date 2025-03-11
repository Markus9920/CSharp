using System.Net;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
namespace PasswordHash;

class Program
{
    static void Main(string[] args)
    {
        PackageManager.InstallSQLPackages();

        UserAccount userAccount = new UserAccount();




        //Lisää toiminnallisuus, jolla käyttäjä ja salasana saadaan tallennettua tietokantaan. Uusi luokka?
        while (true)
        {
            Console.WriteLine("(1) Create account (2) Login");
            string input = Console.ReadLine() ?? String.Empty;

            if (input == String.Empty)
            {
                Console.WriteLine("Empty input!");
                continue;
            }
            switch (input)
            {
                case "1":
                    userAccount.CreateAccout();
                    break;
                case "2":

                    userAccount.Login();
                    break;
            }
        }

    }
}
