using Microsoft.AspNetCore.Mvc;
using SubspaceStats.Areas.League.Models;
using SubspaceStats.Services;

namespace SubspaceStats.Areas.League.Controllers
{
    [Area("League")]
    public class StandingsController(
        ILeagueRepository leagueRepository) : Controller
    {
        private readonly ILeagueRepository _leagueRepository = leagueRepository;

        public IActionResult Index()
        {
            // List of teams with Wins/Losses/Draws record
            // Team Versus to also include Kills For / Kills Against

            //LeagueSeasonStandings standings

            return View();
        }
    }
}
