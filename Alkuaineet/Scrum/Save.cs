using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SCRUM
{

    //no code written by me.
    public static class Save
    {
        public static void SaveAverageToAnswersJson(double average)
        {
            // Define the file path
            string filePath = "Answers.json";
            // Create a new result object
            var result = new
            {
                Date = DateTime.Now.ToString("dd.MM.yyyy"),
                Average = average
            };
            // Save the result to the file in JSON format
            SaveResultsToFile(filePath, result);
            Console.WriteLine("\nYour result has been saved.");
        }

        private static void SaveResultsToFile(string filePath, object result)
        {
            // Initialize a list for results
            var results = new List<object>();

            // Check if the file already exists
            if (File.Exists(filePath))
            {
                // Read the content of the existing file
                string existingData = File.ReadAllText(filePath);

                // Deserialize the content of the existing file from JSON format to a list.
                // If the file content is empty or deserialization fails, initialize a new empty list.
                // ?? is the null-coalescing operator, which returns the right-hand value if the left-hand value is null.
                results = JsonConvert.DeserializeObject<List<object>>(existingData) ?? new List<object>();
            }

            // Add the new result to the list
            results.Add(result);

            // Save the updated result list in JSON format to the file
            File.WriteAllText(filePath, JsonConvert.SerializeObject(results, Formatting.Indented));
        }
    }
}