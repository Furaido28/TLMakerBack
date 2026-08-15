using System.Text;
using API.Configuration;
using API.Services;
using API.Services.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ======================
// 1) Controllers
// ======================
builder.Services.AddControllers();

// ======================
// 2) Swagger / OpenAPI (Configuration Swashbuckle)
// ======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new() { Title = "TLMaker API", Version = "v1" });

    // Configuration pour pouvoir mettre le Token JWT dans Swagger (Bouton "Authorize")
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Entrez 'Bearer' suivi d'un espace et de votre token JWT.\n\nExemple: \"Bearer eyJhbGciOi...\""
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement { {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ======================
// 3) DbContext SQL Server
// ======================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("TLMaker")
    )
);

// ======================
// 4) JWT Configuration
// ======================
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);

var jwt = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "La section 'JwtSettings' est manquante dans appsettings.json"
    );

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.Events = new JwtBearerEvents {
            OnMessageReceived = context => {
                // JWT via cookie (Angular)
                if (context.Request.Cookies.ContainsKey("jwt")) {
                    context.Token = context.Request.Cookies["jwt"];
                }
                // JWT via header (Swagger / Postman)
                else if (context.Request.Headers.ContainsKey("Authorization")) {
                    context.Token = context.Request.Headers["Authorization"]
                        .ToString()
                        .Replace("Bearer ", "");
                }

                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt.SecretKey)
            )
        };
    });

builder.Services.AddAuthorization();

// ======================
// 5) CORS (Angular)
// ======================
const string corsName = "MyOrigins";

builder.Services.AddCors(options => {
    options.AddPolicy(corsName, policy => {
        policy
            .WithOrigins(
                "http://localhost:4200", 
                "http://127.0.0.1:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ======================
// 6) Dependency Injection
// ======================

// Repositories
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

// Services métier
builder.Services.AddScoped<IUsersService, UsersService>();

// Services techniques
builder.Services.AddScoped<IJwtService, JwtService>();

var app = builder.Build();

// ======================
// 7) Pipeline HTTP
// ======================
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TLMaker API v1");
    });
}

app.UseCors(corsName);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();