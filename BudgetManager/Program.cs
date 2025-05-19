using System;
using BudgetManager.Data;
using BudgetManager.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Alusta tietokanta
DatabaseCreator.InitializeDatabase();

// 2️⃣ Lisää CORS fronttia varten (esim. localhost:5500 HTML)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy
            .WithOrigins("http://localhost:5500") // <-- MUISTA vaihtaa oikea portti tarvittaessa
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

// 3️⃣ Lisää palvelut
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddScoped<TokenService>();

var app = builder.Build();

// 4️⃣ Ota staattiset tiedostot käyttöön (esim. JS/CSS)
app.UseStaticFiles();

// 5️⃣ Käytä CORS-politiikkaa frontin suuntaan
app.UseCors("AllowFrontend");

// 6️⃣ HTTPS-uudelleenohjaus
app.UseHttpsRedirection();

// 7️⃣ Ota token-middleware käyttöön
app.UseMiddleware<TokenValidationMiddleware>();

// 8️⃣ Tarvittaessa autentikointi
app.UseAuthentication();
app.UseAuthorization();

// 9️⃣ Käytä kontrollerit
app.MapControllers();

// 🔟 Ei avata Swaggeria – poistettu

app.Run();


/* using System;
using System.Diagnostics;
using BudgetManager.Data;
using BudgetManager.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// initialize database
DatabaseCreator.InitializeDatabase();

// 1️⃣ CORS pitää rekisteröidä ennen Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwagger", policy =>
        policy
            .WithOrigins("http://localhost:5099")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

// 2️⃣ Lisää palvelut
builder.Services.AddControllers()
 .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    }); ;
builder.Services.AddScoped<TokenService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BudgetManager", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Syötä 'Bearer ' ja sitten token. Esim: Bearer eyJhbGciOiJIUzI1Ni..."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 3️⃣ Staattiset tiedostot, jotta Swagger‑UI pystyy lataamaan withCredentials.js
app.UseStaticFiles();

// 4️⃣ Ota CORS‑politiikka käyttöön
app.UseCors("AllowSwagger");

// 5️⃣ Swagger UI konfiguraatio (vain dev‑tilassa)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BudgetManager v1");
        c.InjectJavascript("/swagger-ui/withCredentials.js"); // lisää credentials: 'include'
        c.DefaultModelsExpandDepth(-1); // Tämä estää turhat automaattiset sulkemiset.
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // EI suljeta automaattisesti Responsea
    });
}

// 6️⃣ HTTPS‑redirect
app.UseHttpsRedirection();

// 7️⃣ Token‑validation middleware
app.UseMiddleware<TokenValidationMiddleware>();

// 8️⃣ .NET Core authentication & authorization (tarvittaessa)
app.UseAuthentication();
app.UseAuthorization();

// 9️⃣ Map controller‑reitit
app.MapControllers();

// 10️⃣ Avaa Swagger automaattisesti (valinnainen)
try
{
    Process.Start(new ProcessStartInfo
    {
        FileName = "http://localhost:5099/swagger",
        UseShellExecute = true
    });
}
catch
{
    Console.WriteLine("Open http://localhost:5099/swagger manually");
}

app.Run();
 */