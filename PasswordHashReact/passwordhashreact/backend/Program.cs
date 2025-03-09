using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace PasswordHash;

class Program
{
    static void Main(string[] args)
    {
        UserAccount userAccount = new UserAccount();

        userAccount.CreateAccout();
        
        //Lisää toiminnallisuus, jolla käyttäjä ja salasana saadaan tallennettua tietokantaan. Uusi luokka sille?

    }
}
