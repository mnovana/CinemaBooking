using BlazorCinemaBooking.Components;
using BlazorCinemaBooking.Services;
using BlazorCinemaBooking.Services.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Config;
using System.Text;

namespace BlazorCinemaBooking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            EnvSetup.EnsureEnvFileExists();
            
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                            .AddInteractiveServerComponents();
            builder.Services.AddHttpClient<ShowtimeService>(client => client.BaseAddress = new Uri("https://localhost:5002/gateway/showtimes"));
            builder.Services.AddHttpClient<MovieService>(client => client.BaseAddress = new Uri("https://localhost:5002/gateway/movies"));
            builder.Services.AddHttpClient<UserService>();
            builder.Services.AddScoped<IShowtimeService, ShowtimeService>();
            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddBlazoredLocalStorage();

            // Authentication
            builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

            var issuer = builder.Configuration["JWT_ISSUER"];
            var audiences = builder.Configuration["JWT_AUDIENCE"].Split(',');
            var key = builder.Configuration["JWT_SIGNING_KEY"];
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorComponents<App>()
               .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
