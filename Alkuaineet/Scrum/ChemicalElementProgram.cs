//* -marks the code written by me
//* from here
using System;
using Newtonsoft.Json;
using System.Text.Json;

namespace SCRUM
{
    public class ChemicalElementProgram
    {
        //list for answers
        private List<string> _correctAnswers { get; } 
        private List<string> _wrongAnswers { get; }

        public ChemicalElementProgram() //constructor
        {
            _correctAnswers = new List<string>(); 
            _wrongAnswers = new List<string>();
        }

        public class Result
        {
            public string Date { get; set; }
            public double Average { get; set; }
//*to here
            public Result()
            {
                Date = string.Empty;
            }
        }
//* from here
        public void BeforeStart()
        {

            while (true)
            {
                Console.WriteLine("Press 'P' to start. Press 'R' to view results");

                string? input = Console.ReadLine();
                string answer = input?.ToLower().Trim() ?? string.Empty;

                if (answer == "p")
                {
                    Start();
                    break;
                }
                else if (answer == "r")
                {
                    LookForResults();
                    break;
                }
                else
                {
                    Console.WriteLine("Enter a valid input P/T");
                }
            }
        }
//* to here
        public bool CheckIfContinue(ChemicalElementProgram chemicalElementProgram)
        {
            Console.WriteLine("Enter 'P' if you want to continue || enter anything else if you want to stop");
            // After the game, ask the user if they want to continue. If the user gives 'p' as an answer
            // the while loop continues and the test can be done again.
            string? input = Console.ReadLine();
            string? continueOrNot = input?.Trim().ToLower() ?? string.Empty;
            if (continueOrNot.ToLower() == "p")
            {
                return true;
            }
            else
            {
                chemicalElementProgram.Exit();
                return false;
            }
        }
        public void Start()
        {
            // If the user plays, the program asks the user for five elements from the first 20 elements of the periodic table.
            // The program asks to enter five elements, which are in the text file "alkuaineet.txt".
            // The program checks if the given element is in the list, then the answer is either correct or incorrect.
            Inquiry inquiry = new Inquiry();
            Console.WriteLine("The game starts");
            inquiry.ChemicalElementTest();

            // The user must not enter the same word, so it needs to be checked and temporarily stored. --> HandleTheAnswer
        }

        public void LookForResults()
        {
            // If the user chose to view, the program calculates the current average of the results from the existing result directories and files.
            // Tells the viewer the average.
            Console.WriteLine("Looking at results");

            if (File.Exists("Answers.json"))
            {
                // Fetch the JSON file, which is in the same folder.
                string json = File.ReadAllText("Answers.json");
                // Deserialize the JSON file into a readable list.
                var results = JsonConvert.DeserializeObject<List<Result>>(json);

                if (results != null)
                {
                    // Ask the user if they want to search for a specific date.
                    // Trim removes all extra spaces, ToLower converts the user's input to lowercase.
                    Console.WriteLine("Do you want to search for a specific date? (Y/N)");
                    string response = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;

                    if (response == "y")
                    {
                        // Ask the user for the date.
                        Console.WriteLine("Enter the date (e.g., 10.01.2025):");
                        string date = Console.ReadLine()?.Trim() ?? string.Empty;

                        Console.WriteLine("Here are the results:");

                        var filteredResults = new List<Result>();

                        // Filter the results by the given date.
                        foreach (var result in results)
                        {
                            if (result.Date == date)
                            {
                                filteredResults.Add(result);
                            }
                        }

                        if (filteredResults.Any())
                        {
                            // Print the filtered results.
                            foreach (var result in filteredResults)
                            {
                                Console.WriteLine($"{result.Date} Average = {result.Average}");
                            }
                        }
                        else
                        {
                            // Notify if no results are found for the given date.
                            Console.WriteLine($"No results for the date {date}.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Here are all the results:");
                        // Print all results
                        foreach (var result in results)
                        {
                            Console.WriteLine($"{result.Date} Average = {result.Average}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No results available.");
            }
        }
//* from here
        public void HandleTheAnswer(string answer)
        {
            // The method takes the answer given by the user. Checks if it is correct or incorrect. If it is correct,
            // it is stored in the list of correct answers, if incorrect it is stored in the list of incorrect answers.

            // The file from which the correct answers are fetched.
            string correctAnswersFile = "alkuaineet.txt";
            string path = Path.Combine(Directory.GetCurrentDirectory(), correctAnswersFile);
            try
            {
                string[] lines = File.ReadAllLines(path); // Put the lines of "alkuaineet.txt" into an array.

                if (lines.Contains(answer)) // If found, the answer is correct and it is moved to the list of correct answers.
                {
                    if (!_correctAnswers.Contains(answer))
                    {
                        _correctAnswers.Add(answer);
                    }
                    else
                    {
                        Console.WriteLine("You have already given this answer!");
                    }
                }
                else // If not, it is an incorrect answer and it is put in the list of incorrect answers.
                {
                    if (!_wrongAnswers.Contains(answer))
                    {
                        _wrongAnswers.Add(answer);
                    }
                    else
                    {
                        Console.WriteLine("You have already given this answer!");
                    }
                }
            }
            catch (FileNotFoundException ex) // handling the possible exceptions
            {
                Console.WriteLine("The file alkuaineet.txt is not found. The file may have been deleted from the folder, modified, or it is corrupted.");
                HandleCrash(ex.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected error. Contact support.");
                HandleCrash(e.Message);
            }
        }
//* to here
        public void Average()
        {
            // Calculate the average as a percentage (the proportion of correct answers out of all answers)
            double average = _correctAnswers.Count / 5.0 * 100; // Convert integer to decimal
            Console.WriteLine($"Average (percentage): {average}%");

            // Save the average and date to the Answers.json file
            Save.SaveAverageToAnswersJson(average);
            PrintListItems();
        }
//* from here
        public void PrintListItems() // for testing
        {
            Console.WriteLine("********************Correct answers in the list************");
            foreach (var answer in _correctAnswers)
            {
                Console.WriteLine(answer);
            }

            Console.WriteLine("********************Incorrect answers in the list************");
            foreach (var answer in _wrongAnswers)
            {
                Console.WriteLine(answer);
            }
        }

        public void HandleCrash(string errorMessage) // Method to crash the program.
        {
            Console.WriteLine(errorMessage);
            _correctAnswers.Clear();
            _wrongAnswers.Clear();
            Environment.Exit(1);
        }

        public void Exit()
        {
            Console.WriteLine("Closing the program. Thank you for playing!");
            _correctAnswers.Clear();
            _wrongAnswers.Clear();
            Environment.Exit(1);
        }

        // Getters to get the number of items in the lists, so they can be counted together.
        public int GetCorrectAnswersCount()
        {
            return _correctAnswers.Count();
        }

        public int GetWrongAnswersCount()
        {
            return _wrongAnswers.Count();
        }
    }
}
//* to here
