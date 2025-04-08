
//this is how we can find these folders and the classes they hold
using BudgetManager.Data;
using BudgetManager.Models;
using BudgetManager.Services;

using Microsoft.AspNetCore.Mvc;

namespace budgetManager.Controllers
{
    /*
    ToDo:

    Metodi sisäänkirjautumiselle
    Metodi salasanan vaihtamiselle
    */

    //attributes
    [ApiController] //attribute to give class abitlity to work with API 
    [Route("api/[controller]")] //route to this controller, so API can find it. If class name is UserController, route will be api/user

    public class UserController : ControllerBase //inherits the ControllerBase to get acces to functions needed with web API
    {
        private readonly TokenService _tokenService; // TokenService class from Services folder

        public UserController(TokenService tokenService) //here we inject the class TokenService into this class.
        {
            _tokenService = tokenService;
        }



        [HttpPost] // address for method below
        public IActionResult CreateUser([FromBody] NewUserDTO NewuserDto) //method for creating new user add data tranfer object
        {
            if (string.IsNullOrWhiteSpace(NewuserDto.Username) || string.IsNullOrWhiteSpace(NewuserDto.Password))
            {
                return BadRequest("Please type username and password.");
            }

            var newAccount = new UserAccount(NewuserDto.Username, NewuserDto.Password); //new data tranfer object containing new user info

            bool succes = DatabaseManager.AddUserToDataBase(newAccount.Username, newAccount.Password, newAccount.Salt);

            if (!succes)
            {
                return Conflict("User already exists.");
            }

            return Ok($"{newAccount.Username} created."); // if !succes is false we can do return Ok!
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {

            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Please type username and password.");
            }

            var userId = DatabaseManager.Authentication(loginDto.Username, loginDto.Password);

            if (userId <= 0)
            {
                return Unauthorized("Login error: Invalid credentials.");
            }

            var token = _tokenService.CreateToken(userId);

            return Ok(new { token });

        }
    }
}