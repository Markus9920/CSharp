using Microsoft.Data.Sqlite;

namespace LemmikkiKanta;
//dotnet add package Microsoft.Data.Sqlite

/*  Tee komentorivi sovellus joka: 
-luo lemmikki tietokannan
-Lisää kantaan Omistajia (id, nimi, puhelin)
-Lisää kantaan lemmikkejä(id, nimi, laji, omistajan_id)
-Päivittää omistajan puhelinnumeron
-Etsii lemmikin nimen perusteella omistajan puhelinnumeron
Palauta koodi (Program.cs + muut .cs tiedostot) */


class Program
{
    static void Main(string[] args)
    {
        //luodaan tietokantayhteys

        using (var connection = new SqliteConnection("Data Source=lemmikkikanta.db"))
        {
            connection.Open();

            //Tietikanta taulut Asiakkaat, Tuotteet, Ostokset

            CreateTables(connection);

            while (true)
            {
                //Yksinkertainen komentorivikayttoliittyma

                Console.WriteLine("Mitä haluat tehdä? (1) Lisää omistaja, (2) Lisää lemmikki, (3) Päivitä omistajan puhelinnumero, (4) Hae omistaja (5) Lopeta");
                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        //Lisää omistaja
                        Console.WriteLine("Anna Omistajan nimi:");
                        string ownerName = Console.ReadLine() ?? String.Empty;

                        Console.WriteLine("Anna Omistajan puhelinnumero:");
                        string ownerPhoneNumber = Console.ReadLine() ?? String.Empty;
                        AddPetOwner(connection, ownerName, ownerPhoneNumber);
                        break;

                    case "2":
                        //Lisää lemmikki nimi, laji, omistaja
                        Console.WriteLine("Anna lemmikin nimi:");
                        string petName = Console.ReadLine() ?? String.Empty;

                        //Tässä varmaan pitäisi näyttää listassa olevien omistajien nimet? Jotta käyttäjä voi valita listassa
                        //olevaan omistajan lemmikille.
                        PrintPetOwners(connection).ForEach(Console.WriteLine);
                        Console.WriteLine("Kuka on lemmikin omistaja:");
                        string petOwnerName = Console.ReadLine() ?? String.Empty;

                        AddPetToOwner(connection, petName, petOwnerName);
                        break;

                    case "3":
                        //Päivitä omistajan puhelinnumero
                        break;

                    case "4":
                        //Hae omistajan tiedot puhelinnumeron perusteella
                        break;

                    case "5":
                        connection.Close();
                        return;

                    default:
                        Console.WriteLine("Virheelinen syöte");
                        break;
                }
            }
        }
    }

    static void CreateTables(SqliteConnection conn)
    {
        //Omistaja -taulun luonti
        //Taulu, jossa on omistajan id, nimi, puhelin
        var createTableCommand = conn.CreateCommand();
        createTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS Owners (id INTEGER PRIMARY KEY, name TEXT NOT NULL, phone TEXT NON NULL)";
        createTableCommand.ExecuteNonQuery();

        //Lemmikki taulun luonti id, nimi, laji, omistajan_id
        var createTableCommand2 = conn.CreateCommand();
        createTableCommand2.CommandText = "CREATE TABLE IF NOT EXISTS Pets (id INTEGER PRIMARY KEY, name TEXT NOT NULL, specie TEXT NOT NULL, owner_id INTEGER NOT NULL)";
        createTableCommand2.ExecuteNonQuery();

    }

    static void AddPetOwner(SqliteConnection conn, string ownerName, string phoneNumber)
    {
        //Tämä metodi lisaa omistajan Omistajat tauluun id, nimi, puhelin
        var insertCommand = conn.CreateCommand();
        insertCommand.CommandText = "INSERT INTO Owners (name, phone) VALUES ($name, $phone)";
        insertCommand.Parameters.AddWithValue("$name", ownerName);
        insertCommand.Parameters.AddWithValue("$phone", phoneNumber);
        insertCommand.ExecuteNonQuery();
    }
    static void AddPet(SqliteConnection conn, string petName, string petSpecie)
    {
        //Tama metodi lisaa lemmikin Lemmikit -tauluun id, nimi, laji, omistajan_id

        var insertCommand = conn.CreateCommand();
        insertCommand.CommandText = "INSERT INTO Pets (name, specie) VALUES ($name, $specie)";
        insertCommand.Parameters.AddWithValue("$name", petName);
        insertCommand.Parameters.AddWithValue("$specie", petSpecie);
        insertCommand.ExecuteNonQuery();
    }


    static List<string> PrintPetOwners(SqliteConnection conn)
    {
        List<string> owners = new List<string>();

        //Haetaan asiakkaiden nimet kannasta
        var selectCustomersCommand = conn.CreateCommand();
        selectCustomersCommand.CommandText = "SELECT name FROM Owners";
        var result = selectCustomersCommand.ExecuteReader();

        while (result.Read())
        {
            owners.Add(result.GetString(0));
        }
        return owners;
    }


    static List<string> PrintPets(SqliteConnection conn)
    {
        List<string> pets = new List<string>();

        //Haetaan asiakkaiden nimet kannasta
        var selectProductsCommand = conn.CreateCommand();
        selectProductsCommand.CommandText = "SELECT name FROM Pets";
        var result = selectProductsCommand.ExecuteReader();

        while (result.Read())
        {
            pets.Add(result.GetString(0));
        }

        return pets;
    }

    static void AddPetToOwner(SqliteConnection conn, string petName, string ownerName)
    {
        int ownerId = 0;
        int petId = 0;

        //Haetaan tuotteen id
        var selectCommand = conn.CreateCommand();
        selectCommand.CommandText = "SELECT id FROM Pets WHERE name = $name";
        selectCommand.Parameters.AddWithValue("$name", petName);
        var result = selectCommand.ExecuteReader();

        result.Read();
        petId = result.GetInt32(0);

        //Haetaan omistajan id
        var selectCustomerIdCommand = conn.CreateCommand();
        selectCustomerIdCommand.CommandText = "SELECT id FROM Owners WHERE name = $name";
        selectCustomerIdCommand.Parameters.AddWithValue("$name", ownerName);
        var result2 = selectCustomerIdCommand.ExecuteReader();

        result2.Read();
        ownerId = result2.GetInt32(0);

        //Lisää omistaja_id oikealla lemmikille
        var insertCommand = conn.CreateCommand();
        insertCommand.CommandText = "INSERT INTO Pets (owner_id) VALUES ($owner_id)";
        insertCommand.Parameters.AddWithValue("$owner_id", ownerId);
        insertCommand.ExecuteNonQuery();
    }
}