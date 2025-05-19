using System.Security.Claims;
using BudgetManager.Services;
using BudgetManager.Data;
using BudgetManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class ReferenceDataController : ControllerBase
    {
        [HttpGet("categories")]
        [AllowAnonMiddleware]//no need to be logged in, own made attribute
        public IActionResult GetCategoryEnums()
        {
            var categories = Enum.GetValues(typeof(Category))
                                       .Cast<Category>()
                                       .Where(c => (int)c > 0)
                                       .Select(r => new
                                       {
                                           Id = (int)r,
                                           Name = CategoryManager.FormatCategoryName(r)
                                       })
                                       .ToList();


            return Ok(categories);
        }

        [HttpGet("recurrence-types")]
        [AllowAnonMiddleware]//no need to be logged in, own made attribute
        public IActionResult GetRecurrenceTypes()
        {
            var recurrTypes = Enum.GetValues(typeof(RecurrenceType))
                                       .Cast<RecurrenceType>()
                                       .Select(r => new
                                       {
                                           Id = (int)r,
                                           Name = r.ToString()
                                       })
                                       .ToList();
            return Ok(recurrTypes);
        }
    }
}