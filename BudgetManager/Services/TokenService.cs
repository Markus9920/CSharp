using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Data.Sqlite;


using BudgetManager.Models;
using Microsoft.AspNetCore.Http.HttpResults; //this is how we can find this folder and the classes they hold

namespace BudgetManager.Services;


/* there is a data added to appsettings.json we use to sign the token, we dont want to hardcode it
into our class, so put it there.

"JwtSettings": {
    "TokenKey": "key_to_sign_jwt_token_with_length_of_at_least_32_bytes",
    "Issuer": "BudgetManagerApi",
    "Audience": "BudgetManagerFrontend"
  } 
    we make this class non static, because we want to get access to appsettings.json

    This class handles the function to make token for user who's logged in to our program
    that way we can easily pass userid with API calls, so we know that who was the user that
    is using our program and made the change to our program's database.*/

public class TokenService
{

  private readonly string _key;
  private readonly string _issuer;
  private readonly string _audience;
  public readonly TimeSpan expMinutes = TimeSpan.FromMinutes(60);

  public TokenService(IConfiguration conf) // by using IConfiguration(build in), we can get the key from appsettings.json file.
  {
    _key = conf["JwtSettings:TokenKey"] ?? throw new Exception("Error: key not found"); // if it is found or if it's not found

    _issuer = conf["JwtSettings:Issuer"] ?? throw new Exception("Error: issuer not found");

    _audience = conf["JwtSettings:Audience"] ?? throw new Exception("Error: audience not found");
  }

  public string CreateToken(int userId, string username)
  {
    //takes every character in key string -> to byte -> into array of bytes using UTF8
    SymmetricSecurityKey encodedKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

    //this creates the info for signig the token for logged in user
    //gives info about how to create hash later, when we encrypt the sign for token
    SigningCredentials credentials = new SigningCredentials(encodedKey, SecurityAlgorithms.HmacSha256);

    //Claims about useraccount. This is info we add to token for reading it later.
    var claimArray = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Name, username)
    };

    //now  create the actual token
    var token = new JwtSecurityToken(
      issuer: _issuer, //from who?
      audience: _audience, //to who?
      claims: claimArray, //what are the claims we make about user
      expires: DateTime.UtcNow.Add(expMinutes), //how long until the token expires i.e. how long until the session ends
      signingCredentials: credentials //how to sign the token
    );

    //now we return the hashed and signed token for logged in user.
    return new JwtSecurityTokenHandler().WriteToken(token);

  }

  //this method holds the fukctionality to validate token
  public ClaimsPrincipal? Validate(string token) //method that returns identity information
  {
    //object for reading the token
    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
    byte[] key = Encoding.UTF8.GetBytes(_key); //byte array of the key, that is used to sign the token

    try
    {
      //parameters for how to validate the token
      TokenValidationParameters parameters = new TokenValidationParameters
      {
        ValidateIssuer = true, //check who created the token
        ValidateAudience = true, //check fro who is the token for
        ValidateLifetime = true, //is the token expired
        ValidateIssuerSigningKey = true, //is the token signed using the correct key
        IssuerSigningKey = new SymmetricSecurityKey(key), //key to check if token is signed with the same key created when token was sgined the first time
        ClockSkew = TimeSpan.Zero, //don't allow any overrun of time in tokens expiration
        ValidIssuer = _issuer,
        ValidAudience = _audience
      };

      ClaimsPrincipal principal = handler.ValidateToken(token, parameters, out _); //returns the validated token, but we dont use it that's why _. It is discarded.

      return principal; //this returns user info if token is valid
    }
    catch (Exception)
    {
      return null; //returns null if token is not valid. This is something we can wait to happen. That's why null
    }
  }
  public void SetCookie(HttpResponse response, string token) //this saves the token into cookies
  {
    CookieOptions cookieOptions = new CookieOptions
    {
      HttpOnly = true, //not possible to read using javascript, so there in no haXXors making xss attacks
      Secure = !Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Development") ?? true, //if in development state or published
      SameSite = SameSiteMode.Strict, //this cookie is sent only if its used in this site strictly
      Expires = DateTime.UtcNow.Add(expMinutes)
    };

    //gives instructions to browser for saving the token in cookies
    response.Cookies.Append("jwt", token, cookieOptions);
  }

  public void UpdateToken(HttpRequest request, HttpResponse response, int userId, string username)
  {
    if (!TokenExists(request))
    {
      return;
    }
    RemoveCookie(response);

    string newToken = CreateToken(userId, username);

    SetCookie(response, newToken);
  }

  //this checks if token is found
  public bool TokenExists(HttpRequest request) => request.Cookies.ContainsKey("jwt");

  //this is for removing token
  public void RemoveCookie(HttpResponse response) => response.Cookies.Delete("jwt");
}
