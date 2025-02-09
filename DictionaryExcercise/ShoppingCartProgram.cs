using System;
using System.Collections;
namespace ShoppingCartExcercise
{

    class ShoppingCartProgram
    {
        public ShoppingCartProgram() { }
        public void Start()
        {
            // Variables used in this class.
            bool isInt;
            bool notString;
            int parsedInt;

            ShoppingCart shoppingCart = new ShoppingCart();

            Console.WriteLine("***************************************************************************************************");
            Console.WriteLine("*                                                                                                 *");
            Console.WriteLine("* Welcome to the store. You probably won't leave here satisfied. Well, let's start shopping. *");
            Console.WriteLine("*                                                                                                 *");
            Console.WriteLine("***************************************************************************************************");
            // While loop handles the inquiry from user.
            while (true)
            {

                Console.WriteLine("What do you want to add to the shopping cart?");
                string whatToAdd = Console.ReadLine() ?? string.Empty; // no null warning

                TrimAndLower(whatToAdd);
                notString = int.TryParse(whatToAdd, out parsedInt); // if user gives integer and not string
                shoppingCart.CheckCartContent(whatToAdd);

                if (string.IsNullOrEmpty(whatToAdd))
                {
                    Console.WriteLine("We won't force you to buy anything!\n" +
                    "However, let's print the items currently in the shopping cart:");
                    shoppingCart.PrintCart();
                    break;
                }
                if (notString) // If notString is true
                {
                    Console.WriteLine("We don't sell numbers here!");
                }
                else
                {
                    Console.WriteLine($"Product: {whatToAdd}. How many do you want to add to the shopping cart?");
                    string? howManyToAdd = Console.ReadLine();

                    isInt = int.TryParse(howManyToAdd, out parsedInt);
                    int amount = parsedInt;
                    if (!notString && isInt) // if notString is false and isInt is true, we can go forward
                    {
                        if (whatToAdd.Length <= 1) // Check that user is actually giving a word not just one character.
                        {
                            Console.WriteLine("This isn't a wheel of fortune! Go buy your vowels elsewhere!");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Fool! You gave invalid inputs!");
                    }
                    shoppingCart.AddToCart(whatToAdd, amount);
                }
            }
            Console.WriteLine();
            shoppingCart.MoveDictToListAndPrintAndAfterThatMoveItToArrayAndPrintAgain();
        }

        // Trimming and lowering
        static string TrimAndLower(string text)
        {
            return text.ToLower().Trim();
        }
    }
}