
using Microsoft.EntityFrameworkCore;
using MovieService.Models;
using MovieService.Models.DTO.Mapping;
using MovieService.Repositories;
using MovieService.Repositories.Interfaces;
using SharedLibrary.Config;
using SharedLibrary.Middleware;

namespace MovieService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            EnvSetup.EnsureEnvFileExists();
            
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Database for EF  
            builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(ConfigurationExtensions.GetConnectionString(builder.Configuration, "AppConnectionString")));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
            builder.Services.AddAutoMapper(typeof(MovieProfile));

            // Repositories
            builder.Services.AddScoped<IDirectorRepository, DirectorRepository>();
            builder.Services.AddScoped<IMovieRepository, MovieRepository>();

            // IMiddleware
            builder.Services.AddScoped<GlobalExceptionHandlingMiddleware>();

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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
