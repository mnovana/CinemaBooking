using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SeatReservationService.Models;
using SeatReservationService.Models.DTO.Mapping;
using SeatReservationService.Repositories;
using SeatReservationService.Repositories.Interfaces;
using SeatReservationService.Services;
using SeatReservationService.Services.Interfaces;
using SharedLibrary.Config;
using SharedLibrary.Middleware;
using SharedLibrary.Services.Interfaces;
using SharedLibrary.Services;
using System.Text;
using StackExchange.Redis;

namespace SeatReservationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            EnvSetup.EnsureEnvFileExists();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Database for EF
            string? connectionString = ConfigurationExtensions.GetConnectionString(builder.Configuration, "AppConnectionString");

            builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString));

            // Redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Authentication
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var audiences = Environment.GetEnvironmentVariable("JWT_AUDIENCE").Split(',');
            var key = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudiences = audiences,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                    });
            });

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Repositories
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

            // Services
            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddSingleton<ICacheService, RedisCacheService>();

            // IMiddleware
            builder.Services.AddScoped<GlobalExceptionHandlingMiddleware>();

            // Http clients
            builder.Services.AddHttpClient("ScreeningService", client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SCREENING_SERVICE_URL"));
            });

            builder.Services.AddHttpClient("UserService", client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("USER_SERVICE_URL"));
            });

            // Http context accessor
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
