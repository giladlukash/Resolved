using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Resolved.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resolved.Data
{
    public class DataSeeder
    {
        private ApplicationDbContext _context;

        public DataSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task SeedSuperUser()
        {
            var roleStore = new RoleStore<IdentityRole>(_context);
            var userStore = new UserStore<ApplicationUser>(_context);

            var user = new ApplicationUser
            {
                UserName = "ForumAdmin",
                NormalizedUserName = "forumadmin",
                Email = "resolved@example.com",
                NormalizedEmail = "resolved@example.com",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var hasher = new PasswordHasher<ApplicationUser>();
            var hashedPassword = hasher.HashPassword(user, "123Aa!1");
            user.PasswordHash = hashedPassword;

            var hasAdminRole = _context.Roles.Any(RoleStore => RoleStore.Name == "Admin");
            if (!hasAdminRole)
            {
                roleStore.CreateAsync(new IdentityRole
                { Name = "Admin",
                  NormalizedName = "admin"
                });
            }

            var hasSuperUser = 
                _context.Users.Any(u => u.NormalizedUserName == user.UserName);

            if (!hasSuperUser)
            {
                userStore.CreateAsync(user);
                userStore.AddToRoleAsync(user, "Admin");
            }

            _context.SaveChangesAsync();

            return Task.CompletedTask;
        }
    }
}
