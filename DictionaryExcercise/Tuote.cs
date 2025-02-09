//Luokka Product, josta luodaan ostoskori Dictionaryyn lisätyt tuotteet.
using System;

namespace ShoppingCartExcercise
{
    public class Product
    {
        public string name;

        public int amount;

        public Product(string name)
        {
            this.name = name;
            this.amount = 0;
        }
        public Product(string name, int amount)
        {
            this.name = name;
            this.amount = amount;
        }
        public override string ToString()
        {
            return $"Tuote: {this.name}, Määrä: {this.amount}";
        }
       
    }
}