using System;
namespace ShoppingCartExcercise
{

    class Program
    {
        //Päälooppi on yritetty tehdä melko siistiksi, kaikki asiat tapahtuvat taustalla luokissa.
        static void Main(string[] args)
        {
            //Tällä estetään ettei tähän looppiin palata enää uudestaan.
            bool canStart = false;
            while (!canStart)
            {
                Console.WriteLine("\nOletko valmis aloittamaan ostokset? Y/N");
                string? answerToQuestion = Console.ReadLine() ?? string.Empty; //Osaa odottaa nullia
                

                if (TrimAndLower(answerToQuestion) == "y")
                {
                    canStart = true;
                }
                else if (TrimAndLower(answerToQuestion) == "y")
                {
                    Console.WriteLine("Eihän me väkisin mitään myydä!");
                    break;
                }
                else
                {
                    Console.WriteLine("Anna vastaus Y/N!!!");
                    continue;
                }
            }
            //Saatiin lupa aloittaa.
            if (canStart)
            {
                ShoppingCartProgram program = new ShoppingCartProgram();
                program.Start();
            }
            //Siistitään vähän merkkijonoja.
            static string TrimAndLower(string text)
            {
                return text.ToLower().Trim();
            }
        }
    }
}



