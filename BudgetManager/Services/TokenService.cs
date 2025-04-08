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


using BudgetManager.Models; //this is how we can find this folder and the classes they hold

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

  private readonly int _expMinutes = 60;

  public TokenService(IConfiguration conf) // by using IConfiguration(build in), we can get the key from appsettings.json file.
  {
    _key = conf["JwtSettings:TokenKey"] ?? throw new Exception("Error: key not found"); // if it is found or if it's not found

    _issuer = conf["JwtSettings:Issuer"] ?? throw new Exception("Error: issuer not found");

    _audience = conf["JwtSettings:Audience"] ?? throw new Exception("Error: audience not found");
  }

  public string CreateToken(int userId)
  {
    //takes every character in key string -> to byte -> into array of bytes using UTF8
    var encodedKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

    //this creates the info for signig the token for logged in user
    //gives info about how to create hash later, when we encrypt the sign for token
    var credentials = new SigningCredentials(encodedKey, SecurityAlgorithms.HmacSha256);

    //Claims about useraccount. This is info we add to token for reading it later.
    var claimArray = new[]
    {
        new Claim("userId", userId.ToString())
    };

    //now  create the actual token
    var token = new JwtSecurityToken(
      issuer: _issuer, //from who?
      audience: _audience, //to who?
      claims: claimArray, //what are the claims we make about user
      expires: DateTime.Now.AddMinutes(_expMinutes), //how long until the token expires i.e. how long until the session ends
      signingCredentials: credentials //how to sign the token
    );

    //now we return the hashed and signed token for logged in user.
    return new JwtSecurityTokenHandler().WriteToken(token);

  }
}