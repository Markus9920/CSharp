using Microsoft.AspNetCore.Mvc;
using BudgetManager.Data;
using System.Data;

namespace BudgetManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtherExpensesController : ControllerBase
    {
        // Endpoint to add a new other expense
        [HttpPost("add")]
        public IActionResult AddOtherExpense([FromBody] OtherExpenseDTO expenseDto)
        {
            if (expenseDto == null || string.IsNullOrWhiteSpace(expenseDto.Description) || expenseDto.Amount <= 0)
            {
                return BadRequest("Invalid expense data.");
            }

            // Check if the UserId exists
            //bool userExists = DatabaseManager.DoesUserExist(expenseDto.UserId);
            //if (!userExists)
            //{
                //return BadRequest("User does not exist.");
            //}

            bool success = DatabaseManager.AddOtherExpense(expenseDto.UserId, expenseDto.Description, expenseDto.Amount);

            if (!success)
            {
                return StatusCode(500, "Failed to add expense.");
            }

            return Ok("Expense added successfully.");
        }

        // Endpoint to update an existing other expense
        [HttpPut("update/{expenseId}")]
        public IActionResult UpdateOtherExpense(int expenseId, [FromBody] OtherExpenseDTO expenseDto)
        {
            if (expenseDto == null || string.IsNullOrWhiteSpace(expenseDto.Description) || expenseDto.Amount <= 0)
            {
                return BadRequest("Invalid expense data.");
            }

            bool success = DatabaseManager.UpdateOtherExpense(expenseId, expenseDto.UserId, expenseDto.Description, expenseDto.Amount);

            if (!success)
            {
                return NotFound("Expense not found or update failed.");
            }

            return Ok("Expense updated successfully.");
        }

        // Endpoint to delete an other expense
        [HttpDelete("delete/{expenseId}")]
        public IActionResult DeleteOtherExpense(int expenseId, [FromQuery] int userId)
        {
            bool success = DatabaseManager.DeleteOtherExpense(expenseId, userId);

            if (!success)
            {
                return NotFound("Expense not found or deletion failed.");
            }

            return Ok("Expense deleted successfully.");
        }

        // Endpoint to sum all other expenses for a user
        [HttpGet("sum/{userId}")]
        public IActionResult SumOtherExpenses(int userId)
        {
            double total = DatabaseManager.SumOtherExpenses(userId);

            return Ok(new { Total = total });
        }
    }

    // DTO for other expenses
    public class OtherExpenseDTO
    {
        public int UserId { get; set; }
        public string? Description { get; set; } // Made nullable
        public double Amount { get; set; }
    }
    // End of other expenses controller

    
}