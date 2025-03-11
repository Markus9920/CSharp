using System.Net;

namespace PasswordHash;

class Program
{
    static void Main(string[] args)
    {
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
