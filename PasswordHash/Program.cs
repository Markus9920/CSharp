namespace PasswordHash;

class Program
{
    static void Main(string[] args)
    {
        string username = "";
        string password = "";

        //Query for username
        while (true)
        {
            Console.Write("Give username: ");
            username = Console.ReadLine() ?? String.Empty;

            if (username == String.Empty)
            {
                Console.WriteLine("Give valid username!");
                continue;
            }
            Console.WriteLine();
            break;
        }

        //Query for password
        while (true)
        {
            Console.Write("Give password: ");
            password = Console.ReadLine() ?? String.Empty;

            if (password == String.Empty)
            {
                Console.WriteLine("Give valid password");
                continue;
            }
            Console.WriteLine();
            break;
        }
        //Creates new account
        UserAccount userAccount = new UserAccount(username, password);


        //Lisää toiminnallisuus, jolla käyttäjä ja salasana saadaan tallennettua tietokantaan.

    }
}
