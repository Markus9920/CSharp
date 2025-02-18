using Microsoft.Data.Sqlite;

namespace LemmikkiKanta;
//dotnet add package Microsoft.Data.Sqlite

class Program
{
    public static void Main(string[] args)
    {
        //luodaan tietokantayhteys
        using (var connection = new SqliteConnection("Data Source=lemmikkikanta.db"))
        {
            connection.Open();

            Methods.CreateTables(connection);

            //has no exception handling

            while (true)
            {
                Console.WriteLine("Mitä haluat tehdä? (1) Lisää omistaja, (2) Lisää lemmikki, (3) Lisää lemmikki oikealle omistajalle" +
                " (4) Päivitä omistajan puhelinnumero (5) Hae omistajan puhelinnumero (6) Lopeta (7) debugtulostus");
                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        //Add user
                        Methods.AddPetOwner(connection);
                        break;

                    case "2":
                        //Add pet
                        Methods.AddPet(connection);
                        break;

                    case "3":
                        //Add owner to pet
                        Methods.PrintPets(connection);
                        Methods.PrintPetOwners(connection);
                        Methods.AddPetToOwner(connection);
                        break;

                    case "4":
                        //Update the phone number of the pet owner
                        Methods.PrintPetOwners(connection);
                        Methods.UpdatePhoneNumber(connection);
                        break;

                    case "5":
                        //Get the pet owners phone number by pet name
                        Methods.PrintPets(connection);
                        Methods.GetOwnerPhoneNumberByPetName(connection);
                        break;
                    case "6":
                        connection.Close();
                        return;
                    case "7":
                        Methods.PrintTables(connection);
                        break;
                    default:
                        Console.WriteLine("Virheelinen syöte");
                        break;
                }
            }
        }
    }


}