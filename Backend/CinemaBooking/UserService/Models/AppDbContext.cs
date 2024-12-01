using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace UserService.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Generate IDs and seed
            string userUserId = Guid.NewGuid().ToString();
            string adminUserId = Guid.NewGuid().ToString();
            string userRoleId = Guid.NewGuid().ToString();
            string adminRoleId = Guid.NewGuid().ToString();
            SeedUsers(builder, userUserId, adminUserId);
            SeedRoles(builder, userRoleId, adminRoleId);
            SeedUserRoles(builder, userUserId, adminUserId, userRoleId, adminRoleId);
        }

        private void SeedUsers(ModelBuilder builder, string userUserId, string adminUserId)
        {
            AppUser user = new AppUser()
            {
                Id = userUserId,
                UserName = "novana",
                NormalizedUserName = "NOVANA",
                Email = "novana@gmail.com",
                NormalizedEmail = "NOVANA@GMAIL.COM"
            };

            AppUser admin = new AppUser()
            {
                Id = adminUserId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM"
            };

            PasswordHasher<AppUser> passwordHasher = new PasswordHasher<AppUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, "Novana-0");
            admin.PasswordHash = passwordHasher.HashPassword(admin, "Administrator-0");

            builder.Entity<AppUser>().HasData(user, admin);
        }

        private void SeedRoles(ModelBuilder builder, string userRoleId, string adminRoleId)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = userRoleId, Name = "User", NormalizedName = "USER" },
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" }
            );
        }

        private void SeedUserRoles(ModelBuilder builder, string userUserId, string adminUserId, string userRoleId, string adminRoleId)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = userUserId, RoleId = userRoleId },
                new IdentityUserRole<string> { UserId = adminUserId, RoleId = adminRoleId }
            );
        }
    }
}
