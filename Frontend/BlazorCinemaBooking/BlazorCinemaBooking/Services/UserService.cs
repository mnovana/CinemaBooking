using BlazorCinemaBooking.Models;
using BlazorCinemaBooking.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models.DTO;

namespace BlazorCinemaBooking.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LoginAsync(Login model)
        {
            var response = await _httpClient.PostAsJsonAsync("", model);

            if (!response.IsSuccessStatusCode)
            {
                var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                throw new Exception(problem == null ? "Failed login" : problem.Title);
            }

            var token = await response.Content.ReadFromJsonAsync<TokenDTO>() ?? throw new Exception("Failed to read the token");

            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(2)
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append("jwt_token", token.Token, options);
        }

    }
}
