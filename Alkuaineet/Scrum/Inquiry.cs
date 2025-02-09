namespace SCRUM
{
    // In this file, there is some lines of code made by me
    public class Inquiry
    {
        public void ChemicalElementTest()
        {
            //Create the object that is our actual program
            ChemicalElementProgram chemicalElementProgram = new ChemicalElementProgram();

            //* because user can only answer five times, this is the counter that counts the answers.
            int totalAnswerAmount = chemicalElementProgram.GetCorrectAnswersCount() + chemicalElementProgram.GetWrongAnswersCount();

            Console.WriteLine("Enter five elements: ");

            while (totalAnswerAmount <= 4)//* while total answer count is less or equal to five (list starts at 0)
            {
                string? input = Console.ReadLine();// this can wait for null without givin a warning.
                string? userInput = input?.ToLower().Trim() ?? string.Empty; //* this is able to handle null value, no warning.

                if (userInput != null)
                {
                    chemicalElementProgram.HandleTheAnswer(userInput);
                    totalAnswerAmount = chemicalElementProgram.GetCorrectAnswersCount() + chemicalElementProgram.GetWrongAnswersCount();//* update counter
                }
            }
            chemicalElementProgram.Average();
        }
    }
}