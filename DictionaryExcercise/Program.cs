using System;
namespace ShoppingCartExcercise
{

    class Program
    {
        // Main loop. I wanted to make this as short as I could with my skills.
        static void Main(string[] args)
        {
            
            bool canStart = false; // Cannot return into this loop anymore
            while (!canStart)
            {
                Console.WriteLine("\nAre you ready to start shopping? Y/N");
                string? answerToQuestion = Console.ReadLine() ?? string.Empty; // Can handle null
                
                if (TrimAndLower(answerToQuestion) == "y")
                {
                    canStart = true;
                }
                else if (TrimAndLower(answerToQuestion) == "n")
                {
                    Console.WriteLine("well, we won't force you to buy anything! Now get the **** out of here!");
                    break;
                }
                else
                {
                    Console.WriteLine("Please answer Y/N!!!");
                    continue;
                }
            }
            // Permission to start
            if (canStart)
            {
                ShoppingCartProgram program = new ShoppingCartProgram();
                program.Start();
            }
            // Trimming and lowering
            static string TrimAndLower(string text)
            {
                return text.ToLower().Trim();
            }
        }
    }
}



