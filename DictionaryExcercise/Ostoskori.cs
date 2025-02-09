//Ostoskori -luokka, joka luo uuden Dictionaryn, joka sisältää siihen lisätyt tuotteet(avain) ja niiden määrän(arvo).
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
                //Jos tuote(avain) on jo ostoskorissa alkaa While looppi, joka tekee asioita.
                //Jos määrä(arvo) <= 0, poistetaan tuote. 
                Console.WriteLine("Tuote on jo korissa! Haluatko lisätä(L) vai vähentää(V) määrää? Vai haluatko poistaa(P) tuotteen?\n");

                while (true)
                {
                    string? input = Console.ReadLine() ?? string.Empty;//osaa odottaa nullia
    
                    if (string.IsNullOrEmpty(input)) //Jos käyttäjä antaa tyhjän merkkijonon
                    {
                        Console.WriteLine("Haluatko lisätä(L) vai vähentää(V) määrää? Vai haluatko poistaa(P) tuotteen?");
                    }
                    else if (TrimAndLower(input) == "l")
                    {
                        this.IncreaseAmount(product, AskHowMuch());
                        this.PrintCart();
                        break;
                    }
                    else if (TrimAndLower(input) == "v")
                    {
                        this.ReduceAmount(product, AskHowMuch());
                        this.PrintCart();
                        break;
                    }
                    else if (TrimAndLower(input) == "p")
                    {
                        Console.WriteLine($"{product} poistettu! Ei vissiin kiinnostanu sitte.");
                        this.RemoveFromCart(product);
                        this.PrintCart();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Mitä sää nyt sekoilet?!");
                    }
                }
            }
        }
        //Metodi, jolla lisätään tuotteet dictionaryyns
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
        //Vähennetään arvoa annetun parametrin verran.
        public void ReduceAmount(string product, int amount)
        {
            if (this.shoppingCart.ContainsKey(TrimAndLower(product)))
            {
                int newAmount = this.shoppingCart[product] -= amount;

                if (newAmount <= 0)
                {
                    shoppingCart.Remove(product);
                    Console.WriteLine($"{product} poistettu, koska määrä oli nolla");
                }
            }
        }
        //Lisätään arvoa annetun parametrin verran.
        public void IncreaseAmount(string product, int amount)
        {
            if (this.shoppingCart.ContainsKey(TrimAndLower(product)))
            {
                int newAmount = this.shoppingCart[product] += amount;
            }
        }
        //Kysellään kuinka monta dictionaryyn lisätään.
        public int AskHowMuch()
        {
            bool isInt;
            int parsedInt;

            while (true)
            {
                Console.WriteLine("No montakos ny sitte?");
                string? howMuch = Console.ReadLine();
                isInt = int.TryParse(howMuch, out parsedInt);

                if (!isInt)
                {
                    Console.WriteLine("Torvi! Ei antanut numeroa!");
                }
                else
                {
                    return parsedInt;
                }
            }
        }
        //Poistetaan ostoskorista.
        public void RemoveFromCart(string product)
        {
            this.shoppingCart.Remove(product);
        }
        //Tulostetaan ostoskori.
        public void PrintCart()
        {
            if (IsNullOrEmpty(this.shoppingCart))
            {
                Console.WriteLine("Ei täällä mitäänoo!");
            }
            else
            {
                foreach (KeyValuePair<string, int> kvp in shoppingCart)
                {
                    Console.WriteLine($"Tuote: {kvp.Key}, Määrä: {kvp.Value}");
                }
            }
        }
        //Tarkastetaan onko Dictionary tyhjä vai ei.
        public bool IsNullOrEmpty(Dictionary<string, int> dictionary)
        {
            return (dictionary == null || dictionary.Count < 1);
        }
        //Tämä muuttaa vaan kokonaisluvun merkkijonoksi. Teinpähän nyt tällaisenkin.
        public string IntToString(int amount)
        {
            return amount.ToString();
        }
        //Siirretään Dictionary ensin listaan ja sitten taulukkoon ja tulostellaankin siinä välissä. Kikkailua vaan.
        public void MoveDictToListAndPrintAndAfterThatMoveItToArrayAndPrintAgain()
        {
            Console.WriteLine("*********************************************************");
            Console.WriteLine("Siirrettiin ostetut tuotteet listaan ja tässä ne nyt on");
            foreach (KeyValuePair<string, int> kvp in shoppingCart)
            {
                this.boughtProducts.Add($"Tuote: {kvp.Key} Määrä: {this.IntToString(kvp.Value)}");
            }
            foreach (string line in boughtProducts)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("*********************************************************");


            Console.WriteLine("*********************************************************");
            Console.WriteLine("Siirrettiin ostetut tuotteet taulukkoon ja tässä ne nyt on");
            string[] text = boughtProducts.ToArray();
            foreach (string line in text)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("*********************************************************");
        }
        //Siistitään merkkijonoja.
        static string TrimAndLower(string text)
        {
            return text.ToLower().Trim();
        }
    }
}