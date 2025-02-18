using Microsoft.Data.Sqlite;

namespace LemmikkiKanta;

public static class Methods
{   
    //has no exception handling

    public static void CreateTables(SqliteConnection conn)
    {
        //Creates table for Owners
        //id, name, phone
        var createTableCommand = conn.CreateCommand();
        createTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS Owners (id INTEGER PRIMARY KEY, name TEXT NOT NULL, phone TEXT NON NULL)";
        createTableCommand.ExecuteNonQuery();

        //Creates Pet table
        //id, name, specie, owner_id
        var createTableCommand2 = conn.CreateCommand();
        createTableCommand2.CommandText = "CREATE TABLE IF NOT EXISTS Pets (id INTEGER PRIMARY KEY, name TEXT NOT NULL, specie TEXT NOT NULL, owner_id INTEGER DEFAULT NULL)";
        createTableCommand2.ExecuteNonQuery();

    }
    public static void UpdatePhoneNumber(SqliteConnection conn) //Updates owner phone numbers
    {

        Console.WriteLine("Kenen omistajan numeron haluat vaihtaa?");
        string ownerName = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Anna uusi puhelinnumero");
        string newPhoneNumber = Console.ReadLine() ?? string.Empty;

        string currentPhoneNumber = "";
        string nameFromTable = "";

        //Gets the current phone number by owner's name
        var selectOwnerCommand = conn.CreateCommand();
        selectOwnerCommand.CommandText = "SELECT name, phone FROM Owners WHERE name = $name";
        selectOwnerCommand.Parameters.AddWithValue("$name", ownerName);
        var result = selectOwnerCommand.ExecuteReader();

        result.Read();
        nameFromTable = result.GetString(0);
        currentPhoneNumber = result.GetString(1);
        
        //Updates the old phone number to new number
        var updateCommand = conn.CreateCommand();
        updateCommand.CommandText = "UPDATE Owners SET phone = $new_number WHERE phone = $current";
        updateCommand.Parameters.AddWithValue("$new_number", newPhoneNumber);
        updateCommand.Parameters.AddWithValue("$current", currentPhoneNumber);
        updateCommand.ExecuteNonQuery();

        Console.WriteLine($"{nameFromTable} uusi puhelinnumero on: {newPhoneNumber}");
    }
    public static void GetOwnerPhoneNumberByPetName(SqliteConnection conn) //Gets the owner phone number by pets name
    {
        int ownerId = 0;
        string phoneNumber = "";

        Console.WriteLine("Anna lemmikin nimi.");
        string petName = Console.ReadLine() ?? string.Empty;
    
        //Gets the name of the pet and uses the owner_id to get the owner
        var selectPetCommand = conn.CreateCommand();
        selectPetCommand.CommandText = "SELECT owner_id FROM Pets WHERE name = $petname";
        selectPetCommand.Parameters.AddWithValue("$petname", petName);
        var result = selectPetCommand.ExecuteReader();

        result.Read();
        ownerId = result.GetInt32(0);

        //Once the owner_id is found uses that to get the phone number fron Owners table then prints it
        var selectOwnerCommand = conn.CreateCommand();
        selectOwnerCommand.CommandText = "SELECT phone FROM Owners WHERE id = $owner_id";
        selectOwnerCommand.Parameters.AddWithValue("$owner_id", ownerId);
        var result2 = selectOwnerCommand.ExecuteReader();

        result2.Read();
        phoneNumber = result2.GetString(0);

        Console.WriteLine($"Omistajan puhelinnumero: {phoneNumber}");

    }
    //***********************Add methods *************************************************************************************************************************
    public static void AddPetOwner(SqliteConnection conn) //Adds owner to Owners table
    {
        Console.WriteLine("Anna Omistajan nimi:");
        string ownerName = Console.ReadLine() ?? String.Empty;

        Console.WriteLine("Anna Omistajan puhelinnumero:");
        string phoneNumber = Console.ReadLine() ?? String.Empty;

        var insertCommand = conn.CreateCommand();
        insertCommand.CommandText = "INSERT INTO Owners (name, phone) VALUES ($name, $phone)";
        insertCommand.Parameters.AddWithValue("$name", ownerName);
        insertCommand.Parameters.AddWithValue("$phone", phoneNumber);
        insertCommand.ExecuteNonQuery();
    }
    public static void AddPet(SqliteConnection conn) //Adds pet to Pets table
    {
        Console.WriteLine("Anna Lemmikin nimi:");
        string petName = Console.ReadLine() ?? String.Empty;

        Console.WriteLine("Mitä lajia lemmikki edustaa?:");
        string specie = Console.ReadLine() ?? String.Empty;

        //Only adds pet with name and specie, owmer_id is null for now
        var insertCommand = conn.CreateCommand();
        insertCommand.CommandText = "INSERT INTO Pets (name, specie) VALUES ($name, $specie)";
        insertCommand.Parameters.AddWithValue("$name", petName);
        insertCommand.Parameters.AddWithValue("$specie", specie);
        insertCommand.ExecuteNonQuery();
    }
    public static void AddPetToOwner(SqliteConnection conn) //Connects owner and pet together
    {
        Console.WriteLine("Anna lemmikin nimi:");
        string petName = Console.ReadLine() ?? String.Empty;

        Console.WriteLine("Kuka on lemmikin omistaja:");
        string ownerName = Console.ReadLine() ?? String.Empty;

        int ownerId = 0; //holds the owner id found from table
        int petId = 0; //holds the pet id found from table

        //Gets the pet id from Pets table and saves it into variable petId
        var selectPetCommand = conn.CreateCommand();
        selectPetCommand.CommandText = "SELECT id FROM Pets WHERE name = $name";
        selectPetCommand.Parameters.AddWithValue("$name", petName);
        var result = selectPetCommand.ExecuteReader();

        result.Read();
        petId = result.GetInt32(0);

        Console.WriteLine($"Lemmikin id: {petId}***");

        //Gets the owner id from Owners table and saves it into variable ownerId
        var selectOwnerCommand = conn.CreateCommand();
        selectOwnerCommand.CommandText = "SELECT id FROM Owners WHERE name = $name";
        selectOwnerCommand.Parameters.AddWithValue("$name", ownerName);
        var result2 = selectOwnerCommand.ExecuteReader();

        result2.Read();
        ownerId = result2.GetInt32(0);

        Console.WriteLine($"Omistajan id: {ownerId}***");

        //Makes the owner_id in Pets table to be same as id in the Owners table
        var updateCommand = conn.CreateCommand();
        updateCommand.CommandText = "UPDATE Pets SET owner_id = $owner_id WHERE id = $pet_id";
        updateCommand.Parameters.AddWithValue("$owner_id", ownerId);
        updateCommand.Parameters.AddWithValue("$pet_id", petId);
        updateCommand.ExecuteNonQuery();
    }
    //*******************************************************************************************************************************************************************
    //***********************Print methods ******************************************************************************************************************************
    public static void PrintPetOwners(SqliteConnection conn)
    {
        //Prints the table from database
        var selectOwnersCommand = conn.CreateCommand();
        selectOwnersCommand.CommandText = "SELECT id, name, phone FROM Owners";
        var result = selectOwnersCommand.ExecuteReader();

        //Reads the table and prints all content
        Console.WriteLine("Tietokannassa olevat omistajat:");
        while (result.Read())
        {
            Console.WriteLine($"ID: {result.GetInt32(0)}, name: {result.GetString(1)}, phone: {result.GetString(2)}");
        }
    }
    public static void PrintPets(SqliteConnection conn)
    {
        //Prints the table from database
        var selectPetsCommand = conn.CreateCommand();
        selectPetsCommand.CommandText = "SELECT id, name, specie, owner_id FROM Pets";
        var result = selectPetsCommand.ExecuteReader();

        Console.WriteLine("Tietokannassa olevat lemmikit");
        while (result.Read())
        {
            //Prints null, if table has no content in owner id in column
            int? ownerId = result.IsDBNull(3) ? (int?)null : result.GetInt32(3);
            Console.WriteLine($"ID: {result.GetInt32(0)}, name: {result.GetString(1)}, specie: {result.GetString(2)}, owner ID: {ownerId}");
        }
    }
    
    public static void PrintTables(SqliteConnection conn)
    {
        //Prints all tables from database
        var selectOwnersCommand = conn.CreateCommand();
        selectOwnersCommand.CommandText = "SELECT id, name, phone FROM Owners";
        var result = selectOwnersCommand.ExecuteReader();

        Console.WriteLine("Tietokannassa olevat omistajat:");
        while (result.Read())
        {
            Console.WriteLine($"ID: {result.GetInt32(0)}, name: {result.GetString(1)}, phone: {result.GetString(2)}");
        }

        var selectPetsCommand = conn.CreateCommand();
        selectPetsCommand.CommandText = "SELECT id, name, specie, owner_id FROM Pets";
        var result2 = selectPetsCommand.ExecuteReader();

        Console.WriteLine("Tietokannassa olevat lemmikit");
        while (result2.Read())
        {
            //Prints null, if table has no content in owner id in column
            int? ownerId = result2.IsDBNull(3) ? (int?)null : result2.GetInt32(3);
            Console.WriteLine($"ID: {result2.GetInt32(0)}, name: {result2.GetString(1)}, specie: {result2.GetString(2)}, owner ID: {ownerId}");
        }
    }
}