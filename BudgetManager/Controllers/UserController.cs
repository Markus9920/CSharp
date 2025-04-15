//this is how we can find these folders and the classes they hold
using System.Security.Claims;
using BudgetManager.Data;
using BudgetManager.Models;
using BudgetManager.Services;

using Microsoft.AspNetCore.Mvc;

namespace budgetManager.Controllers
{
    /*
        ToDo ja muistio:

        Luokkaroolien muistisääntö:
        Controller = vastaanottaa HTTP-pyynnön ja palauttaa vastauksen
        Service = tekee varsinaisen sovelluslogiikan (esim. tokenin luonti/tarkastus)
        Repository / Manager = hakee ja tallentaa tietoa esim. tietokannasta
        DTO = kuljettaa tietoa sisään ja ulos (esim. JSON-objekti)
        => Yksi luokka, yksi vastuu – ei sekoiteta tehtäviä
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

        //endpoint
        [HttpPost] // address for method below
        public IActionResult CreateUser([FromBody] NewUserDTO newUserDto) //method for creating new user add data tranfer object
        {
            if (string.IsNullOrWhiteSpace(newUserDto.Username) || string.IsNullOrWhiteSpace(newUserDto.Password))
            {
                return BadRequest("Please type username and password.");
            }

            UserAccount newAccount = new UserAccount(newUserDto.Username, newUserDto.Password); //new data transfer object containing new user info

            bool succes = DatabaseManager.AddUserToDataBase(newAccount.Username, newAccount.GetPass(), newAccount.GetSalt());

            if (!succes)
            {
                return Conflict("User already exists.");
            }

            return Ok($"{newAccount.Username} created."); // if !succes is false we can do return Ok!
        }

        //endpoint
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            //if there is old token still existing, we delete it
            if (_tokenService.TokenExists(Request))
            {
                _tokenService.RemoveCookie(Response);
            }

            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password)) // if empty input
            {
                return BadRequest("Please type username and password.");
            }

            int userId = DatabaseManager.Authentication(loginDto.Username, loginDto.Password); //user id from database using own made authentication method

            if (userId <= 0) //user id cannot be 0 or less
            {
                return Unauthorized("Login error: Invalid credentials.");
            }

            string token = _tokenService.CreateToken(userId, loginDto.Username);

            _tokenService.SetCookie(Response, token);

            return Ok(new { token }); //returns the token
        }

        //endpoint
        [HttpPost("logout")] //Method to log out
        public IActionResult Logout()
        {
            if (!_tokenService.TokenExists(Request))
            {
                return BadRequest("No token found");
            }

            _tokenService.RemoveCookie(Response);
            return Ok("Succesfully logged out");
        }

        //endpoint
        [HttpPost("change-password")]//method to change password
        public IActionResult ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            if (String.IsNullOrWhiteSpace(changePasswordDTO.Username) ||
            String.IsNullOrWhiteSpace(changePasswordDTO.OldPassword) ||
            String.IsNullOrWhiteSpace(changePasswordDTO.NewPassword))
            {
                return BadRequest("All fields required.");
            }

            string? token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token cannot be found.");
            }

            ClaimsPrincipal? principal = _tokenService.Validate(token);

            if (principal == null)
            {
                return Unauthorized("Token is expired or it is invalid.");
            }

            //string is empty or string cannot be parsed to int, as id numers are in database
            string? userIdAsString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (String.IsNullOrEmpty(userIdAsString) || !int.TryParse(userIdAsString, out int userId))//also converts userIdAsString => userId
            {
                return Unauthorized("Missing or invalid token.");
            }

            bool success = DatabaseManager.ChangePassword(userId, changePasswordDTO.OldPassword!, changePasswordDTO.NewPassword!);

            if (!success)
            {
                return Unauthorized("Incorrect password or user does not exists");
            }

            _tokenService.UpdateToken(Request, Response, userId, changePasswordDTO.Username);

            return Ok("New password changed succesfully.");
        }
    }
}