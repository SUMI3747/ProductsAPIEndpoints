using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PrdouctsApi.Data;
using ProductInventoryManagerAPI.Data;
using ProductInventoryManagerAPI.Extensions;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Retrieve JWT Settings from Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/detailedLogs.txt",
                  rollingInterval: RollingInterval.Hour,
                  restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
    .CreateLogger();

builder.Host.UseSerilog();

// Configure Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//}
// Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    options.AddPolicy("SalesAndAdmin", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.IsInRole("admin") || context.User.IsInRole("sales"));
    });
});

// Register Custom Services
builder.Services.AddProductIntialApiServices();

// Configure Database Contexts
var connectionString = builder.Configuration.GetConnectionString("cs");
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<UserCredetialsContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error"); 
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); 

app.Run();
