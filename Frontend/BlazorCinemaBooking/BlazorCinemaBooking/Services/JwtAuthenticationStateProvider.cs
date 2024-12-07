using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace BlazorCinemaBooking.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IConfiguration _configuration;

        public JwtAuthenticationStateProvider(ILocalStorageService localStorageService, IConfiguration configuration)
        {
            _localStorageService = localStorageService;
            _configuration = configuration;
        }
        
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageService.GetItemAsStringAsync("jwt_token");
            token = token?.Trim('"');

            if (string.IsNullOrEmpty(token) || !TokenIsValid(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "Bearer");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        private bool TokenIsValid(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var key = _configuration["JWT_SIGNING_KEY"];
                var issuer = _configuration["JWT_ISSUER"];
                var audiences = _configuration["JWT_AUDIENCE"].Split(',');

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudiences = audiences,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                }, out _);

                return true;
            }
            catch
            {
                return false;
            }

        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = WebEncoders.Base64UrlDecode(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }
    }
}
