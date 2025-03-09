namespace PasswordHash;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Write("Give username:");
            string username = Console.ReadLine() ?? String.Empty;

            if (username == String.Empty)
            {
                Console.WriteLine("Give valid username!");
            }
        }

        

    }
}
