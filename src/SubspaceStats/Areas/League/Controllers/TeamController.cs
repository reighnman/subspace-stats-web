using Microsoft.AspNetCore.Mvc;
using SubspaceStats.Areas.League.Models;
using SubspaceStats.Areas.League.Models.Team;
using SubspaceStats.Services;

namespace SubspaceStats.Areas.League.Controllers
{
    [Area("League")]
    public class TeamController(
        ILeagueRepository leagueRepository) : Controller
    {
        private readonly ILeagueRepository _leagueRepository = leagueRepository;

        public async Task<IActionResult> Index(long teamId, CancellationToken cancellationToken)
        {
            TeamWithSeasonInfo? teamInfo = await _leagueRepository.GetTeamsWithSeasonInfosync(teamId, cancellationToken);
            if(teamInfo is null)
            {
                return NotFound();
            }

            // TODO: Current Standing (Wins, Losses, Draws,...)
            // TODO: Roster + Stats of each player

            return View(
                new TeamViewModel()
                {
                    TeamInfo = teamInfo,
                    GameRecords = await _leagueRepository.GetTeamGames(teamId, cancellationToken),
                });
        }
    }
}
