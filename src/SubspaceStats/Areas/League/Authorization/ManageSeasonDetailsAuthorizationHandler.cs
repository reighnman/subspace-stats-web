using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SubspaceStats.Areas.Identity.Data;
using SubspaceStats.Areas.League.Models.Season;
using SubspaceStats.Services;

namespace SubspaceStats.Areas.League.Authorization
{
    public class ManageSeasonDetailsAuthorizationHandler(
        UserManager<SubspaceStatsUser> userManager,
        ILeagueRepository leagueRepository)
        : AuthorizationHandler<ManagerRequirement, SeasonDetails>
    {
        private readonly UserManager<SubspaceStatsUser> _userManager = userManager;
        private readonly ILeagueRepository _leagueRepository = leagueRepository;

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ManagerRequirement requirement,
            SeasonDetails resource)
        {
            if (context.User is null)
            {
                return;
            }

            if (context.User.IsInRole(RoleNames.Administrator))
            {
                context.Succeed(requirement);
                return;
            }

            string? userId = _userManager.GetUserId(context.User);
            if (userId is null)
            {
                return;
            }

            if (await _leagueRepository.IsLeagueOrSeasonManager(userId, resource.SeasonId, CancellationToken.None))
            {
                context.Succeed(requirement);
            }
        }
    }
}
