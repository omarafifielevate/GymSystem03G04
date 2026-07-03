using GymSystem.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL
{
    public static class IdentityDataSeeder
    {
        public static async Task SeedIdentityDataAsyc(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, CancellationToken ct = default)
        {
            bool hasUsers = await userManager.Users.AnyAsync(ct);
            bool hasRoles = await roleManager.Roles.AnyAsync(ct);

            if (hasUsers || hasRoles) return;

            var roles = new List<IdentityRole>
            {
                new IdentityRole("SuperAdmin"),
                new IdentityRole("Admin")
            };

            foreach (var role in roles)
            {
                var roleResult = await roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                {
                    return;
                }
            }

            var user01 = new ApplicationUser()
            {
                Email = "omar@gmail.com",
                FirstName = "Omar",
                LastName = "Afifi",
                PhoneNumber = "01068080700",
                UserName = "omarafifi"
            };

            await userManager.CreateAsync(user01, "P@ssw0rd");
            await userManager.AddToRoleAsync(user01, "SuperAdmin");

            //User 02
            var user02 = new ApplicationUser()
            {
                Email = "ali@gmail.com",
                FirstName = "Ali",
                LastName = "Hesham",
                PhoneNumber = "01068088700",
                UserName = "alihesham"
            };

            await userManager.CreateAsync(user02, "P@ssw0rd");
            await userManager.AddToRoleAsync(user02, "Admin");





        }
    }
}
