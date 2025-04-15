
using System.Security.Claims;
using BudgetManager.Services;
using Microsoft.AspNetCore.Mvc;


namespace budgetManager.Controllers
{
    [ApiController] //attribute to give class abitlity to work with API 
    [Route("api/[controller]")] //route to this controller, so API can find it. If class name is UserController, route will be api/user

    public class TokenValificationController : ControllerBase //inherits the ControllerBase to get acces to functions needed with web API
    {
        private readonly TokenService _tokenService; // TokenService class from Services folder

        public TokenValificationController(TokenService tokenService) //here we inject the class TokenService into this class.
        {
            _tokenService = tokenService;
        }


        //this method is used to validate token.
        //endpoint
        [HttpGet("Validate")]
        public IActionResult ValidateToken()
        {
            string? token = Request.Cookies["jwt"]; //this gets the token from cookies

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token not found.");
            }

            var user = _tokenService.Validate(token); //method from TokenService class

            if (user == null)
            {
                return Unauthorized("Token expired or invalid.");
            }

            return Ok("Token valid.");
        }
    }
}