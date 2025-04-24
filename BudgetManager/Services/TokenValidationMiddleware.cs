using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Security.Claims;
using System.Threading.Tasks;


//This is a class that checks if token is not expired and it's valid before we do any API calls
//It works between API and front and only gives us passage to API if token is ok

//we have register this class in program.cs 
namespace BudgetManager.Services
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        public TokenValidationMiddleware(RequestDelegate next) //injection
        {
            _next = next;

        }

        //This is a method that checks if token is valid and waits for answer, then it gives the permission to go on
        //It can handle any kind of API call
        public async Task InvokeAsync(HttpContext context)
        {
            //Because we have API methods, that do not require user to be logged in, we have to exclude those 
            var endpoint = context.GetEndpoint(); //this is the endpoint of the API call

            //checks if own made attribute is used in API call
            bool skipAuth = endpoint?.Metadata.GetMetadata<AllowAnonMiddlewareAttribute>() != null;

            if (skipAuth) // if true (!= null)
            {
                await _next(context);
                return;
            }

            // TokenService is a scoped service and cannot be injected into middleware constructor.
            // Instead, we resolve it from the request's service provider to ensure it's tied to the current request.
            var tokenService = context.RequestServices.GetRequiredService<TokenService>();

            //first we get tokens from cookies
            string? accesToken = tokenService.GetAccesToken(context.Request);
            string? refreshToken = tokenService.GetRefreshToken(context.Request);

            //Check for access token is not null
            if (!string.IsNullOrEmpty(accesToken))
            //every time user makes new API call, it means that user is active, so we can create new access token if we still have valis refresh token existing.
            {
                //if token is existing, we try to validate it
                var principal = tokenService.ValidateAccesToken(accesToken);
                if (principal != null)
                {
                    //User now has the principal to use API. This is for ASP.NET, so it knows who User is
                    context.User = principal;
                    //if all is good, we can move on and make API call
                    await _next(context);
                    return;
                }

            }
            //if refresh token is not null
            if (!string.IsNullOrEmpty(refreshToken))//if refresh token is still existing
            //acces token can be null, then we just check if refresh token is existing
            {

                var principal = tokenService.ValidateRefreshToken(refreshToken);

                if (principal != null)
                {
                    //we need new info for acces token if it is expired, but user is still active(makes api calls)

                    string? userIdAsString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value; //user id
                    string? username = principal.FindFirst(ClaimTypes.Name)?.Value;//username   

                    //if user id can be parsed to int it can be used as id number
                    if (int.TryParse(userIdAsString, out int userId) && !string.IsNullOrEmpty(username))
                    {
                        //if refresh token is valid and still existing, we can refresh access token, if user is still active
                        string? newAccessToken = tokenService.RefreshAccesToken(context.Request, context.Response, userId, username);

                        if (!string.IsNullOrEmpty(newAccessToken))
                        {
                            var newPrincipal = tokenService.ValidateAccesToken(newAccessToken);
                            
                            if (newPrincipal != null)
                            {
                                context.User = newPrincipal;
                                await _next(context);
                                return;
                            }
                        }
                    }
                }
            }
            context.Response.StatusCode = 401; //error code 401 Unauthorized
            await context.Response.WriteAsync("Access token expired or invalid");
        }
    }
}