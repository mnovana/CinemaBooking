using BlazorCinemaBooking.Models;
using BlazorCinemaBooking.Services.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models.DTO;

namespace BlazorCinemaBooking.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;

        public UserService(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:5002");
            _localStorageService = localStorageService;
        }

        public async Task LoginAsync(Login model)
        {
            var response = await _httpClient.PostAsJsonAsync("gateway/users/login", model);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed login: {response.StatusCode}");
            }

            var token = await response.Content.ReadFromJsonAsync<TokenDTO>() ?? throw new Exception("Failed to read the token");

            await _localStorageService.SetItemAsync("jwt_token", token.Token);
        }

        public async Task LogoutAsync()
        {
            await _localStorageService.RemoveItemAsync("jwt_token");
        }

    }
}
