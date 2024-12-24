using Application.Services.Implements;
using Application.Services.Interfaces;
using Application.Utilities.AutoMapper;
using Domain.Interfaces.IRepository;
using infrastructure.Data;
using infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyProject.Infrastructure.RabbitMQ;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilog پیکربندی
string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()  // نوشتن به کنسول
    .WriteTo.File(Path.Combine(logDirectory, "log-.txt"), rollingInterval: RollingInterval.Day) 
    .CreateLogger();

builder.Logging.ClearProviders();  
builder.Logging.AddSerilog();

//Add dbContext
builder.Services.AddDbContext<DataContext>(option =>
{

    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));

}
);


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// AutoMapper
builder.Services.AddAutoMapper(typeof(MapperConfig));

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
        ValidAudience = builder.Configuration["JWTSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWTSettings:Key"]))
    };
});



// افزودن سرویس‌های RabbitMQ به DI
builder.Services.AddSingleton<IHostedService, RabbitMQMessageBroker>();


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();
app.MapControllers();

app.Run();
