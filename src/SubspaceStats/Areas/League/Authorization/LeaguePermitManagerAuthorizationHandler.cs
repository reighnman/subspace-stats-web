using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SubspaceStats.Areas.Identity.Data;
using SubspaceStats.Areas.League.Models.League;
using SubspaceStats.Services;

namespace SubspaceStats.Areas.League.Authorization
{
    public class LeaguePermitManagerAuthorizationHandler(
        UserManager<SubspaceStatsUser> userManager,
        ILeagueRepository leagueRepository)
        : AuthorizationHandler<PermitManagerRequirement, LeagueModel>
    {
        private readonly UserManager<SubspaceStatsUser> _userManager = userManager;
        private readonly ILeagueRepository _leagueRepository = leagueRepository;

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermitManagerRequirement requirement,
            LeagueModel resource)
        {
            if (context.User is null)
            {
                return;
            }

            string? userId = _userManager.GetUserId(context.User);
            if (userId is null)
            {
                return;
            }

            if (await _leagueRepository.IsUserLeaguePermitManager(userId, resource.Id, CancellationToken.None))
            {
                context.Succeed(requirement);
            }
        }
    }
}
