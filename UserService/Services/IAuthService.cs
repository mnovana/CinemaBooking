using Microsoft.AspNetCore.Identity;
using UserService.Models.DTO;

namespace UserService.Services
{
    public interface IAuthService
    {
        public Task<IdentityResult> RegisterAsync(RegisterDTO model);
        public Task<TokenDTO?> LoginAsync(LoginDTO model);
        public Task<bool> EmailExistsAsync(string email);
    }
}
