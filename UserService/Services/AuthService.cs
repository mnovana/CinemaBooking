using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Models;
using UserService.Models.DTO;

namespace UserService.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AuthService> _logger;
        private readonly AppDbContext _context;

        public AuthService(UserManager<AppUser> userManager, ILogger<AuthService> logger, AppDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public async Task<TokenDTO?> LoginAsync(LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user != null)
            {
                if (await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    return await GenerateTokenAsync(user);
                }
                else
                {
                    _logger.LogWarning("Wrong password for '{username}'.", model.Username);
                }
            }
            else
            {
                _logger.LogWarning("Wrong username '{username}'.", model.Username);
            }

            return null;
        }

        private async Task<TokenDTO> GenerateTokenAsync(AppUser user)
        {
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            // Get roles for the user
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY");
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.Now.AddHours(2),      // token valid for 2 hours
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new TokenDTO
            {
                Username = user.UserName,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDTO model)
        {
            AppUser user = new AppUser
            {
                UserName = model.Username,
                NormalizedUserName = model.Username.ToUpper(),
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper()
            };

            var userResult = await _userManager.CreateAsync(user, model.Password);

            if(!userResult.Succeeded)
            {
                var userErrors = string.Join(" ", userResult.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to add new user '{username}': {errors}", model.Username, userErrors);

                return userResult;
            }

            _logger.LogInformation("New user '{username}' added.", model.Username);

            var roleResult = await _userManager.AddToRoleAsync(user, "User");

            if(!roleResult.Succeeded)
            {
                var roleErrors = string.Join(" ", roleResult.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to add user '{username}' to role 'User': {errors}", model.Username, roleErrors);

                var deleteResult = await _userManager.DeleteAsync(user);

                if(!deleteResult.Succeeded)
                {
                    var deleteErrors = string.Join(" ", deleteResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to delete user '{username}': {errors}", model.Username, deleteErrors);
                    _logger.LogWarning("User '{username}' has no roles.", model.Username);
                }
                else
                {
                    _logger.LogInformation("User '{username}' deleted.", model.Username);
                }

                return roleResult;
            }

            return IdentityResult.Success;
        }
    }
}
