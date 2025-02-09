using System;
using System.Collections;
namespace ShoppingCartExcercise
{

    class ShoppingCartProgram
    {
        public ShoppingCartProgram() { }
        public void Start()
        {

            bool isInt;
            bool notString;
            int parsedInt;
            ShoppingCart shoppingCart = new ShoppingCart();
            Console.WriteLine("***************************************************************************************************");
            Console.WriteLine("*                                                                                                 *");
            Console.WriteLine("*Tervetuloa kauppaan. Täältä et todennäköisesti lähde tyytyväisenä. Noniin, aloitetaanpa ostokset.*");
            Console.WriteLine("*                                                                                                 *");
            Console.WriteLine("***************************************************************************************************");
            //While looppi, jossa kysytään mitä käyttäjä haluaa lisätä ostoskoriin.
            while (true)
            {
                //Lisää tähän tarkastus onko avain jo sanakirjassa
                Console.WriteLine("Mitä haluat lisätä ostoskoriin?");
                string whatToAdd = Console.ReadLine() ?? string.Empty;//osaa odottaa nullia

                TrimAndLower(whatToAdd);
                notString = int.TryParse(whatToAdd, out parsedInt); //Jos käyttäjä antaakin kokonaisluvun, eikä merkkijonoa.
                shoppingCart.CheckCartContent(whatToAdd);

                if (string.IsNullOrEmpty(whatToAdd))
                {
                    Console.WriteLine("Eihän me väkisin mitään myydä!\n" +
                    "Tulostetaan ny kuitenkin vielä ostoskorissa olevat tuotteet:");
                    shoppingCart.PrintCart();
                    break;
                }
                if (notString) //jos notString == tosi
                {
                    Console.WriteLine("Ei me täällä mitään numeroita myydä!");
                }
                else
                {
                    Console.WriteLine($"Tuote: {whatToAdd}. Montako haluat lisätä ostoskoriin.");
                    string? howManyToAdd = Console.ReadLine();

                    isInt = int.TryParse(howManyToAdd, out parsedInt);
                    int amount = parsedInt;
                    if (!notString && isInt)//Jos notStrin == epätosi ja isInt == tosi, voidaan jatkaa eteenpäin.
                    {
                        if (whatToAdd.Length <= 1)//Tarkastaa käyttäjän antaman merkkijonon pituuden, jotta käyttäjä antaisi oikeasti jonkun sanan.
                        {
                            Console.WriteLine("Ei tää mikään onnenpyörä oo! Osta vokaalis muualta!");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Torvi! Annoit epäkelpoja syötteitä!");
                    }
                    shoppingCart.AddToCart(whatToAdd, amount);
                }
            }
            Console.WriteLine();
            shoppingCart.MoveDictToListAndPrintAndAfterThatMoveItToArrayAndPrintAgain();
            //Tässä piti olla vielä mahdollisuus siirtää ostoskorin sisältä tekstitiedostoon, mutta ei vaan tullut vaikka yritti.
        }

        //Tällekin luokalle oma siistimismetodi, niin on metodinkutsu vähän mukavamman näköinen.
        static string TrimAndLower(string text)
        {
            return text.ToLower().Trim();
        }
    }
}