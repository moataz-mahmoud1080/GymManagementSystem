using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.DAL.Data.DataSeeding
{
    public static class IdentityDataSeading
    {
        public static async Task SeedAsync(
            RoleManager<IdentityRole> roleManager ,
            UserManager<ApplicationUser> userManager ,
            ILogger logger,
            CancellationToken ct =default)
        {
            try
            {
                bool HasUser = await userManager.Users.AnyAsync();
                bool HasRole = await roleManager.Roles.AnyAsync();
                if (HasRole && HasUser) return;
                if(! HasRole)
                {
                    //Create Two Roles
                    var Roles = new List<IdentityRole>()
                    {
                         new IdentityRole() {Name = "SuperAdmin"},
                         new IdentityRole() {Name = "Admin"}
                    };
                    foreach (var role in Roles)
                    {
                        if(! await roleManager.RoleExistsAsync(role.Name!))
                        {
                            var result = await roleManager.CreateAsync(role);
                            if (!result.Succeeded)
                             logger.LogError($"Failed To Create Role {role.Name}");                           
                        }
                    }                  
                }

                if(!HasUser)
                {
                    //Create SuperAdmin User
                    var superAdmin = new ApplicationUser()
                    {
                        FirstName ="Moataz",
                        LastName ="Mahmoud",
                        UserName = "MoatazMahmoud",
                        Email ="moatazMahmoud108@gmail.com",
                        PhoneNumber ="01207410081",
                    };
                   await  userManager.CreateAsync(superAdmin, "P@ssw0rd");
                   await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");

                    //Create Admin User
                    var admin = new ApplicationUser()
                    {
                        FirstName ="Mohamed",
                        LastName ="Mahmoud",
                        UserName = "MohamedMahmoud",
                        Email ="mhamedMahmoud99@gmail.com",
                        PhoneNumber ="01229244276",
                    };
                   await  userManager.CreateAsync(admin, "P@ssw0rd");
                   await userManager.AddToRoleAsync(admin, "Admin");


                }
                return;
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Identity Seeding Failed");
                return;
            
            }

        }
    }
}
