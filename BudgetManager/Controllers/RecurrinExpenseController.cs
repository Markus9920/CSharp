using System.Security.Claims;
using BudgetManager.Services;
using BudgetManager.Data;
using BudgetManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;




namespace BudgetManager.Controllers
{
    [ApiController] //attribute to give class abitlity to work with API 
    [Route("api/[controller]")] //route to this controller, so API can find it.
    public class RecurringExpenseController : ControllerBase //inherits the ControllerBase to get acces to functions needed with web API
    {
        private readonly TokenService _tokenService; // TokenService class from Services folder

        public RecurringExpenseController(TokenService tokenService) //here we inject the class TokenService into this class.
        {
            _tokenService = tokenService;
        }

        [HttpPost("add-recurr-expense")]
        public IActionResult AddRecurringExpense([FromBody] RecurrExpenseDTO recurrExpenseDto)
        {
            //Gets the use id from access token
            int? userId = _tokenService.GetUserIdFromAccesToken(Request);

            //if any of this information is null
            if (userId == null || string.IsNullOrWhiteSpace(recurrExpenseDto.Description) || recurrExpenseDto.Cost == null ||
            recurrExpenseDto.OccurrDate == default)
            {
                return BadRequest("Invalid or null data");
            }

            Category categoryEnum;
            string normalizedCategoryName = string.Empty;

            if (string.IsNullOrWhiteSpace(recurrExpenseDto.Category))
            {
                categoryEnum = Category.None;
            }
            else
            {
                //return category name back to enum name format
                normalizedCategoryName = recurrExpenseDto.Category?.Replace(" ", "_") ?? "";
                if (!Enum.TryParse<Category>(normalizedCategoryName, true, out categoryEnum))
                {
                    return BadRequest("Invalid category type");
                }
            }


            //Here we try to convert Json answer from swagger to Enum typ
            if (!Enum.TryParse<RecurrenceType>(recurrExpenseDto.RecurrenceType, true, out var recurrenceTypeEnum))
            {
                return BadRequest("Invalid recurrence type");
            }

            DateTime nextOccurenceDate = RecurExpenseService.SetCorrectNextOccuranceType(recurrExpenseDto.OccurrDate,
            recurrenceTypeEnum);

            bool success = RecurringExpensesManager.AddRecurringExpense(userId.Value, recurrExpenseDto.Description!, recurrExpenseDto.Cost ?? 0,
            recurrenceTypeEnum, recurrExpenseDto.OccurrDate, nextOccurenceDate, false, categoryEnum);

            if (!success)
            {
                return Conflict("Error adding expense to database");
            }

            return Ok($"Expense added successfully"); // if !success is false we can do return Ok!
        }

        //Get the expenses that are coming for that month
        [HttpGet("get-all-unpaid-recurr")]
        public IActionResult GetUnpaidRecurringExpenses()
        {

            int? userId = _tokenService.GetUserIdFromAccesToken(Request);

            if (userId == null)
            {
                return Unauthorized("Invalid user id or user unauthorized");
            }

            var unPaidExpenses = RecurringExpensesManager.GetUnpaidRecurrExpensesFromDb(userId.Value);

            return Ok(unPaidExpenses);
        }
        //Mark the expense paid

        //Get all the expenses
        [HttpGet("get-all-recurr")]
        public IActionResult GetAllRecurringExpenses()
        {
            int? userId = _tokenService.GetUserIdFromAccesToken(Request);

            if (userId == null)
            {
                return Unauthorized("Invalid user id or user unauthorized");
            }

            var allExpenses = RecurringExpensesManager.GetAllRecurrExpensesFromDb(userId.Value);

            return Ok(allExpenses);

        }
        [HttpPost("mark-paid-recurr")]
        public IActionResult MarkRecurrExpensePaid([FromBody] ExpenseIdDTO expenseIdDTO)
        {
            //Method to set isPaid => true, so the expense is marked as paid    
            int? userId = _tokenService.GetUserIdFromAccesToken(Request);

            if (userId == null)
            {
                return Unauthorized("Invalid user id or user unauthorized");
            }

            bool success = RecurringExpensesManager.MarkRecurrExpensePaid(expenseIdDTO.ExpenseId, userId.Value);

            if (!success)
            {
                return NotFound("Expense not found or already paid");
            }

            return Ok("Expense marked as paid");
        }
        //Mark the expense not paid
        [HttpPost("mark-not-paid-recurr")]
        public IActionResult MarkRecurrExpenseNotPaid([FromBody] ExpenseIdDTO expenseIdDTO)
        {
            //Method to set isPaid => true, so the expense is marked as paid    
            int? userId = _tokenService.GetUserIdFromAccesToken(Request);

            if (userId == null)
            {
                return Unauthorized("Invalid user id or user unauthorized");
            }

            bool success = RecurringExpensesManager.MarkRecurrExpenseNotPaid(expenseIdDTO.ExpenseId, userId.Value);

            if (!success)
            {
                return NotFound("Expense not found or already marked unpaid");
            }

            return Ok("Expense marked as unpaid");
        }
        //Delete expense
        [HttpDelete("delete-recurr-expense")]
        public IActionResult DeleteRecurringExpense([FromBody] ExpenseIdDTO expenseIdDTO)
        {
            int? userId = _tokenService.GetUserIdFromAccesToken(Request);

            if (userId == null)
            {
                return Unauthorized("Invalid user id or user unauthorized");
            }

            bool success = DatabaseManager.DeleteExpense(expenseIdDTO.ExpenseId, userId.Value, DatabaseManager.recurringExpenseTableName);

            if (!success)
            {
                return NotFound("Expense not found");
            }
            return Ok("Expense deleted successfully");
        }

        //Get the sum for expenses
        [HttpGet("get-sum-recurr")]
        public IActionResult GetSumOfRecurrExpenses()
        {
            int? userId = _tokenService.GetUserIdFromAccesToken(Request);

            if (userId == null)
            {
                return Unauthorized("Invalid user id or user unauthorized");
            }

            double totalExpenseCost = DatabaseManager.SumExpenses(userId.Value, DatabaseManager.recurringExpenseTableName);

            return Ok($"Sum of expenses {totalExpenseCost}");
        }
        //Update the expense
        [HttpPost("update-recurr-expense")]
        public IActionResult UpdateRecurringExpense([FromBody] ExpenseEditDTO expenseEditDTO)
        {
            //method to update description, cost or next occurence date of the expense

            int? userId = _tokenService.GetUserIdFromAccesToken(Request);

            if (userId == null)
            {
                return Unauthorized("Invalid user id or user unauthorized");
            }

            if (expenseEditDTO.ExpenseId == null || string.IsNullOrWhiteSpace(expenseEditDTO.NewDescription) ||
                expenseEditDTO.NewCostValue == null || expenseEditDTO.NewNextOccurrDate == null)
            {
                return BadRequest("Invalid or null data");
            }

            bool success = RecurringExpensesManager.UpdateRecurringExpense(userId.Value, expenseEditDTO.ExpenseId.Value,
                expenseEditDTO.NewDescription, expenseEditDTO.NewCostValue.Value, expenseEditDTO.NewNextOccurrDate.Value);

            if (!success)
            {
                return NotFound("Expense not found or update failed");
            }

            return Ok("Expense updated successfully");
        }
    }
}