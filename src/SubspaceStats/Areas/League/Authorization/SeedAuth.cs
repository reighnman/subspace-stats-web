using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SubspaceStats.Areas.Identity.Data;
using SubspaceStats.Options;
using System.Text;

namespace SubspaceStats.Areas.League.Authorization
{
    public static class SeedAuth
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            // Create the Administrator role if it doesn't exist.
            if (!await EnsureRole(serviceProvider, RoleNames.Administrator))
            {
                return;
            }

            IOptions<LeagueOptions>? leagueOptions = serviceProvider.GetRequiredService<IOptions<LeagueOptions>>();
            if (leagueOptions.Value.Seed is null
                || string.IsNullOrWhiteSpace(leagueOptions.Value.Seed.AdminUsername)
                || string.IsNullOrWhiteSpace(leagueOptions.Value.Seed.AdminPassword))
            {
                return;
            }

            // Get or create the user.
            SubspaceStatsUser user = await EnsureUser(serviceProvider, leagueOptions.Value.Seed.AdminUsername, leagueOptions.Value.Seed.AdminPassword);

            // Assign the Administrator role to the user if it's not yet assigned.
            await EnsureUserRole(serviceProvider, user, RoleNames.Administrator);
        }

        private static async Task<bool> EnsureRole(IServiceProvider serviceProvider, string role)
        {
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(role));
                if (!result.Succeeded)
                {
                    throw CreateExceptionFromIdentityErrors(result);
                }
            }

            return true;
        }

        private static async Task<SubspaceStatsUser> EnsureUser(
            IServiceProvider serviceProvider,
            string userName,
            string password) 
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<SubspaceStatsUser>>();

            SubspaceStatsUser? user = await userManager.FindByNameAsync(userName);
            if (user is null)
            {
                user = new SubspaceStatsUser
                {
                    UserName = userName,
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    throw CreateExceptionFromIdentityErrors(result);
                }
            }

            return user;
        }

        private static async Task EnsureUserRole(IServiceProvider serviceProvider, SubspaceStatsUser user, string role)
        {
            UserManager<SubspaceStatsUser> userManager = serviceProvider.GetRequiredService<UserManager<SubspaceStatsUser>>();

            if (!await userManager.IsInRoleAsync(user, role))
            {
                IdentityResult result = await userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    throw CreateExceptionFromIdentityErrors(result);
                }
            }
        }

        private static Exception CreateExceptionFromIdentityErrors(IdentityResult result)
        {
            StringBuilder sb = new();
            foreach (var error in result.Errors)
            {
                sb.AppendLine($"{error.Code} - {error.Description}");
            }

            return new Exception(sb.ToString());
        }
    }
}
