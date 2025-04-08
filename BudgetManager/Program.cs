using BudgetManager.Data;
using BudgetManager.Services;
using BudgetManager.Models;
using System.Diagnostics;
using Swashbuckle.AspNetCore.SwaggerGen;

DatabaseCreator.InitializeDatabase();

var builder = WebApplication.CreateBuilder(args); //Builder object. Gathers all the necessary options and services


// Add services to the container.
builder.Services.AddControllers(); //Registers controller classes.
builder.Services.AddScoped<TokenService>(); //TokenService class from Services folder


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

//these are for swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();//This creates the WebAplication object that is the actual program

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); //Directs all the Http calls into Https for safety

app.UseAuthorization(); //Makes possible to check authorizations.

app.MapControllers(); //Routes all the http calls to controllers


//This opens the swagger ui in browser, so there is no need to do it manually every time.
var url = "http://localhost:5099/swagger";
try
{
    Process.Start(new ProcessStartInfo
    {FileName = url,
    UseShellExecute = true
    });
}
catch
{
    Console.WriteLine("Error, open swagger ui manually {url}");
}


app.Run(); //Runs the program

//use this in web browser when running
//http://localhost:5099/swagger // of if this is not the correct address, check the console
/* for example. This is the first line in console after "Building..." 
info: Microsoft.Hosting.Lifetime[14]
Now listening on: http://localhost:5099  just add /swagger to the address */