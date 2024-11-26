using BlazorCinemaBooking.Components;
using BlazorCinemaBooking.Services;
using BlazorCinemaBooking.Services.Interfaces;
using Blazored.LocalStorage;

namespace BlazorCinemaBooking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                            .AddInteractiveServerComponents();
            builder.Services.AddHttpClient<ShowtimeService>(client => client.BaseAddress = new Uri("https://localhost:5002/gateway/showtimes"));
            builder.Services.AddHttpClient<MovieService>(client => client.BaseAddress = new Uri("https://localhost:5002/gateway/movies"));
            builder.Services.AddHttpClient<UserService>(client => client.BaseAddress = new Uri("https://localhost:5002/gateway/users"));
            builder.Services.AddScoped<IShowtimeService, ShowtimeService>();
            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddBlazoredLocalStorage();

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

            app.MapRazorComponents<App>()
               .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
