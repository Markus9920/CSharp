// ShoppingCart class that creates a new Dictionary containing the added products (key) and their quantities (value).
using System;
using System.Formats.Asn1;

namespace ShoppingCartExcercise
{
    public class ShoppingCart
    {
        private Dictionary<string, int> shoppingCart;

        private List<string> boughtProducts;

        public ShoppingCart()
        {
            this.shoppingCart = new Dictionary<string, int>();

            this.boughtProducts = new List<string>();
        }
        public void CheckCartContent(string product)
        {
            if (this.shoppingCart.ContainsKey(product))
            {
                // If the product (key) is already in the shopping cart, a while loop starts that performs actions.
                // If the quantity (value) is <= 0, the product is removed.

                Console.WriteLine("The product is already in the cart! Do you want to add (A) or reduce (R) the quantity? Or do you want to remove (D) the product?\n");

                while (true)
                {
                    string? input = Console.ReadLine() ?? string.Empty;//This is able to handle null, no warning

                    if (string.IsNullOrEmpty(input)) //If user gives empty string
                    {
                        Console.WriteLine("Do you want to add (A) or reduce (R) the quantity? Or do you want to remove (D) the product?");
                    }
                    else if (TrimAndLower(input) == "a")
                    {
                        this.IncreaseAmount(product, AskHowMuch());
                        this.PrintCart();
                        break;
                    }
                    else if (TrimAndLower(input) == "r")
                    {
                        this.ReduceAmount(product, AskHowMuch());
                        this.PrintCart();
                        break;
                    }
                    else if (TrimAndLower(input) == "d")
                    {
                        Console.WriteLine($"{product} removed! Seems like you weren't interested.");
                        this.RemoveFromCart(product);
                        this.PrintCart();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("What the **** are you doing?!");
                    }
                }
            }
        }
        //Method to add stuff into dictionary
        public void AddToCart(string product, int amount)
        {
            if (!this.shoppingCart.ContainsKey(product))
            {
                shoppingCart.Add(product, amount);
                this.PrintCart();
            }
            else
            {
                CheckCartContent(product);
            }


        }
        //Reduce the amount by given parameter
        public void ReduceAmount(string product, int amount)
        {
            if (this.shoppingCart.ContainsKey(TrimAndLower(product)))
            {
                int newAmount = this.shoppingCart[product] -= amount;

                if (newAmount <= 0)
                {
                    shoppingCart.Remove(product);
                    Console.WriteLine($"{product} removed because the quantity was zero");
                }
            }
        }
        //Increase the amount in dictionary
        public void IncreaseAmount(string product, int amount)
        {
            if (this.shoppingCart.ContainsKey(TrimAndLower(product)))
            {
                int newAmount = this.shoppingCart[product] += amount;
            }
        }
        //Asks how many of that product user wants to add into dictionary.
        public int AskHowMuch()
        {
            bool isInt;
            int parsedInt;

            while (true)
            {
                Console.WriteLine("How many do you want?");
                string? howMuch = Console.ReadLine();
                isInt = int.TryParse(howMuch, out parsedInt);

                if (!isInt)
                {
                    Console.WriteLine("Fool! You didn't give a number!");
                }
                else
                {
                    return parsedInt;
                }
            }
        }
        //Removes from dictionary
        public void RemoveFromCart(string product)
        {
            this.shoppingCart.Remove(product);
        }
        //Prints the dictionary
        public void PrintCart()
        {
            if (IsNullOrEmpty(this.shoppingCart))
            {
                Console.WriteLine("There's nothing here!");
            }
            else
            {
                foreach (KeyValuePair<string, int> kvp in shoppingCart)
                {
                    Console.WriteLine($"Product: {kvp.Key}, Quantity: {kvp.Value}");
                }
            }
        }
        //Check if dictionary is empty
        public bool IsNullOrEmpty(Dictionary<string, int> dictionary)
        {
            return (dictionary == null || dictionary.Count < 1);
        }
        //Converts int into string, just to mess around with code.
        public string IntToString(int amount)
        {
            return amount.ToString();
        }
        //This is also messing around. Moves dictionary into list and into array. Then prints them. 
        public void MoveDictToListAndPrintAndAfterThatMoveItToArrayAndPrintAgain()
        {
            Console.WriteLine("*********************************************************");
            Console.WriteLine("Moved purchased products to a list and here they are");
            foreach (KeyValuePair<string, int> kvp in shoppingCart)
            {
                this.boughtProducts.Add($"Product: {kvp.Key} Quantity: {this.IntToString(kvp.Value)}");
            }
            foreach (string line in boughtProducts)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("*********************************************************");


            Console.WriteLine("*********************************************************");
            Console.WriteLine("Moved purchased products to an array and here they are");
            string[] text = boughtProducts.ToArray();
            foreach (string line in text)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("*********************************************************");
        }
        //Trimming and lowering.
        static string TrimAndLower(string text)
        {
            return text.ToLower().Trim();
        }
    }
}