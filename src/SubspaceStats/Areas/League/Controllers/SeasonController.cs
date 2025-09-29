using Microsoft.AspNetCore.Mvc;
using SubspaceStats.Areas.League.Models;
using SubspaceStats.Areas.League.Models.Season;
using SubspaceStats.Areas.League.Models.Team;
using SubspaceStats.Services;

namespace SubspaceStats.Areas.League.Controllers
{
    [Area("League")]
    public class SeasonController(
        ILogger<SeasonController> logger,
        ILeagueRepository leagueRepository,
        IStatsRepository statsRepository) : Controller
    {
        private readonly ILogger<SeasonController> _logger = logger;
        private readonly ILeagueRepository _leagueRepository = leagueRepository;
        private readonly IStatsRepository _statsRepository = statsRepository;

        // GET Season/5
        public async Task<ActionResult> IndexAsync(long id, CancellationToken cancellationToken)
        {
            // link to League Details: /League/Season/<season id>/Details
            // TODO: link to manage teams for league/season admins: /League/Season/<season id>/Teams
            // TODO: link to manage games for league/season admins: /League/Season/<season id>/Games
            // TODO: link to edit season for league/season admins

            Task<List<LeagueWithSeasons>> leagueWithSeasonsTask = _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            Task<List<ScheduledGame>> scheduledGamesTask = _leagueRepository.GetScheduledGamesAsync(id, cancellationToken);
            Task<List<TeamStanding>> standingsTask = _leagueRepository.GetSeasonStandingsAsync(id, cancellationToken);
            // TODO: completed games

            await Task.WhenAll(leagueWithSeasonsTask, scheduledGamesTask, standingsTask);

            return View(
                new SeasonViewModel()
                {
                    LeagueSeasonChooser = new LeagueSeasonChooserViewModel(id, leagueWithSeasonsTask.Result, Url),
                    ScheduledGames = scheduledGamesTask.Result,
                    Standings = standingsTask.Result,
                });
        }

        // GET Season/5/Rosters
        public async Task<ActionResult> Rosters(long id, CancellationToken cancellationToken)
        {
            // TODO: link to League Details
            // TODO: link to manage teams for league/season admins
            // TODO: link to manage games for league/season admins

            Task<List<LeagueWithSeasons>> leagueWithSeasonsTask = _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            Task<List<SeasonRoster>?> getSeasonRostersTask = _leagueRepository.GetSeasonRostersAsync(id, cancellationToken);
            
            await Task.WhenAll(leagueWithSeasonsTask, getSeasonRostersTask);

            return View(
                new SeasonRostersViewModel()
                {
                    LeagueSeasonChooser = new LeagueSeasonChooserViewModel(id, leagueWithSeasonsTask.Result, Url),
                    Rosters = getSeasonRostersTask.Result ?? [],
                });
        }

        // GET Season/5/Details
        public async Task<ActionResult> Details(long id, CancellationToken cancellationToken)
        {
            SeasonDetails? details = await _leagueRepository.GetSeasonDetailsAsync(id, cancellationToken);
            if (details is null)
            {
                return NotFound();
            }

            Task<List<LeagueWithSeasons>> leagueWithSeasonsTask = _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            Task<OrderedDictionary<long, string>> gameTypeTask = _statsRepository.GetGameTypesAsync(cancellationToken);

            await Task.WhenAll(leagueWithSeasonsTask, gameTypeTask);

            return View(
                new SeasonDetailsViewModel
                {
                    LeagueSeasonChooser = new LeagueSeasonChooserViewModel(id, leagueWithSeasonsTask.Result, Url),
                    Details = details,
                    GameTypes = gameTypeTask.Result,
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Start(long id, DateTime? startDate, CancellationToken cancellationToken)
        {
            await _leagueRepository.StartSeasonAsync(id, startDate, cancellationToken);
            return RedirectToAction("Details");
        }

        // GET Season/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST Season/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET Season/5/Edit
        public ActionResult Edit(long id)
        {
            return View();
        }

        // POST Season/5/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(long id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET Season/5/Copy
        public async Task<ActionResult> Copy(long id, CancellationToken cancellationToken)
        {
            return View();
        }

        // POST Season/5/Copy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CopyConfirm(long id, CancellationToken cancellationToken)
        {
            // The view model will need to pass:
            // id of season to copy
            // season_name
            // p_include_players boolean
	        // p_include_teams boolean
            // p_include_games boolean
	        // p_include_rounds boolean
            return View();
        }

        // GET Season/5/Delete
        public ActionResult Delete(long id)
        {
            return View();
        }

        // POST Season/5/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(long id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Players(long? id, CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            Task<List<LeagueWithSeasons>> leagueWithSeasonsTask = _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            Task<List<PlayerModel>> playersTask = _leagueRepository.GetSeasonPlayersAsync(id.Value, cancellationToken);
            Task<List<TeamModel>> teamsTask = _leagueRepository.GetSeasonTeamsAsync(id.Value, cancellationToken);

            await Task.WhenAll(leagueWithSeasonsTask, playersTask);

            return View(
                new PlayersViewModel
                {
                    LeagueSeasonChooser = new LeagueSeasonChooserViewModel(id.Value, leagueWithSeasonsTask.Result, Url),
                    Players = playersTask.Result,
                    Teams = teamsTask.Result,
                });
        }

        public async Task<ActionResult> Teams(long? id, CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            Task<List<LeagueWithSeasons>> leagueWithSeasonsTask = _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            Task<List<TeamModel>> teamsTask = _leagueRepository.GetSeasonTeamsAsync(id.Value, cancellationToken);

            await Task.WhenAll(leagueWithSeasonsTask, teamsTask);

            return View(
                new TeamsViewModel
                {
                    LeagueSeasonChooser = new LeagueSeasonChooserViewModel(id.Value, leagueWithSeasonsTask.Result, Url),
                    Teams = teamsTask.Result,
                });
        }

        public async Task<ActionResult> Games(long? id, CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            Task<List<LeagueWithSeasons>> leagueWithSeasonsTask = _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            Task<List<GameListItem>> gamesTask = _leagueRepository.GetSeasonGamesAsync(id.Value, cancellationToken);
            Task<List<TeamModel>> teamsTask = _leagueRepository.GetSeasonTeamsAsync(id.Value, cancellationToken);

            await Task.WhenAll(leagueWithSeasonsTask, gamesTask, teamsTask);

            return View(
                new SeasonGamesViewModel
                {
                    LeagueSeasonChooser = new LeagueSeasonChooserViewModel(id.Value, leagueWithSeasonsTask.Result, Url),
                    Games = gamesTask.Result,
                    Teams = teamsTask.Result,
                });
        }
    }
}
