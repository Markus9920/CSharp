
using System;

namespace ShoppingCartExcercise
{
    public class Product
    {
        //Product class. This is the object that is to be added in dictionary.
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
            return $"Product: {this.name}, Amount: {this.amount}";
        }
       
    }
}