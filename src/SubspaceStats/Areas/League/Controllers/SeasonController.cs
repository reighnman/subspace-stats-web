using Microsoft.AspNetCore.Mvc;
using SubspaceStats.Areas.League.Models;
using SubspaceStats.Areas.League.Models.Season;
using SubspaceStats.Areas.League.Models.Team;
using SubspaceStats.Models;
using SubspaceStats.Services;

namespace SubspaceStats.Areas.League.Controllers
{
    [Area("League")]
    public class SeasonController(
        ILeagueRepository leagueRepository,
        IStatsRepository statsRepository) : Controller
    {
        private readonly ILeagueRepository _leagueRepository = leagueRepository;
        private readonly IStatsRepository _statsRepository = statsRepository;

        // GET Season/5
        public async Task<ActionResult> IndexAsync(long? seasonId, CancellationToken cancellationToken)
        {
            if (seasonId is null)
            {
                return NotFound();
            }

            // link to League Details: /League/Season/<season id>/Details
            // TODO: link to manage teams for league/season admins: /League/Season/<season id>/Teams
            // TODO: link to manage games for league/season admins: /League/Season/<season id>/Games
            // TODO: link to edit season for league/season admins

            Task<List<LeagueWithSeasons>> leagueWithSeasonsTask = _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            Task<List<ScheduledGame>> scheduledGamesTask = _leagueRepository.GetScheduledGamesAsync(seasonId.Value, cancellationToken);
            Task<List<TeamStanding>> standingsTask = _leagueRepository.GetSeasonStandingsAsync(seasonId.Value, cancellationToken);
            // TODO: completed games

            await Task.WhenAll(leagueWithSeasonsTask, scheduledGamesTask, standingsTask);

            return View(
                new SeasonViewModel()
                {
                    LeagueSeasonChooser = new LeagueSeasonChooserViewModel(seasonId.Value, leagueWithSeasonsTask.Result, Url),
                    ScheduledGames = scheduledGamesTask.Result,
                    Standings = standingsTask.Result,
                });
        }

        // GET Season/5/Rosters
        public async Task<ActionResult> Rosters(long seasonId, CancellationToken cancellationToken)
        {
            // TODO: link to League Details
            // TODO: link to manage teams for league/season admins
            // TODO: link to manage games for league/season admins

            Task<List<LeagueWithSeasons>> leagueWithSeasonsTask = _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            Task<List<SeasonRoster>?> getSeasonRostersTask = _leagueRepository.GetSeasonRostersAsync(seasonId, cancellationToken);
            
            await Task.WhenAll(leagueWithSeasonsTask, getSeasonRostersTask);

            return View(
                new SeasonRostersViewModel()
                {
                    LeagueSeasonChooser = new LeagueSeasonChooserViewModel(seasonId, leagueWithSeasonsTask.Result, Url),
                    Rosters = getSeasonRostersTask.Result ?? [],
                });
        }

        // GET Season/5/Details
        public async Task<ActionResult> Details(long seasonId, CancellationToken cancellationToken)
        {
            SeasonDetails? details = await _leagueRepository.GetSeasonDetailsAsync(seasonId, cancellationToken);
            if (details is null)
            {
                return NotFound();
            }

            Task<List<LeagueWithSeasons>> leagueWithSeasonsTask = _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            Task<OrderedDictionary<long, GameType>> gameTypeTask = _statsRepository.GetGameTypesAsync(cancellationToken);

            await Task.WhenAll(leagueWithSeasonsTask, gameTypeTask);

            return View(
                new SeasonDetailsViewModel
                {
                    LeagueSeasonChooser = new LeagueSeasonChooserViewModel(seasonId, leagueWithSeasonsTask.Result, Url),
                    Details = details,
                    GameTypes = gameTypeTask.Result,
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Start(long seasonId, DateTime? startDate, CancellationToken cancellationToken)
        {
            await _leagueRepository.StartSeasonAsync(seasonId, startDate, cancellationToken);
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
        public ActionResult Edit(long seasonId)
        {
            return View();
        }

        // POST Season/5/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(long seasonId, IFormCollection collection)
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
        public async Task<ActionResult> Copy(long seasonId, CancellationToken cancellationToken)
        {
            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(seasonId, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            Task<List<PlayerListItem>> playersTask = _leagueRepository.GetSeasonPlayersAsync(seasonId, cancellationToken);
            Task<List<TeamModel>> teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId, cancellationToken);
            Task<List<GameListItem>> gamesTask = _leagueRepository.GetSeasonGamesAsync(seasonId, cancellationToken);
            Task<List<SeasonRound>> roundsTask = _leagueRepository.GetSeasonRoundsAsync(seasonId, cancellationToken);
            await Task.WhenAll(playersTask, teamsTask, gamesTask, roundsTask);

            return View(
                new CopySeasonViewModel
                {
                    SourceSeason = seasonDetails,
                    SourcePlayers = playersTask.Result,
                    SourceTeams = teamsTask.Result,
                    SourceGames = gamesTask.Result,
                    SourceRounds = roundsTask.Result,
                    SeasonName = "",
                });
        }

        // POST Season/5/Copy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Copy(
            long seasonId, 
            [Bind("SeasonName", "IncludePlayers", "IncludeTeams", "IncludeGames", "IncludeRounds")] CopySeasonViewModel copyInfo, 
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(seasonId, cancellationToken);
                if (seasonDetails is null)
                {
                    return NotFound();
                }

                Task<List<PlayerListItem>> playersTask = _leagueRepository.GetSeasonPlayersAsync(seasonId, cancellationToken);
                Task<List<TeamModel>> teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId, cancellationToken);
                Task<List<GameListItem>> gamesTask = _leagueRepository.GetSeasonGamesAsync(seasonId, cancellationToken);
                Task<List<SeasonRound>> roundsTask = _leagueRepository.GetSeasonRoundsAsync(seasonId, cancellationToken);
                await Task.WhenAll(playersTask, teamsTask, gamesTask, roundsTask);

                copyInfo.SourceSeason = seasonDetails;
                copyInfo.SourcePlayers = playersTask.Result;
                copyInfo.SourceTeams = teamsTask.Result;
                copyInfo.SourceGames = gamesTask.Result;
                copyInfo.SourceRounds = roundsTask.Result;

                return View(copyInfo);
            }

            long newSeasonId = await _leagueRepository.CopySeasonAsync(
                seasonId,
                copyInfo.SeasonName,
                copyInfo.IncludePlayers,
                copyInfo.IncludeTeams,
                copyInfo.IncludeGames,
                copyInfo.IncludeRounds,
                cancellationToken);

            return RedirectToAction("Details", "Season", new { seasonId = newSeasonId });
        }

        // GET Season/5/Delete
        public ActionResult Delete(long seasonId)
        {
            return View();
        }

        // POST Season/5/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(long seasonId, IFormCollection collection)
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

        public async Task<ActionResult> Players(long? seasonId, CancellationToken cancellationToken)
        {
            if (seasonId is null)
            {
                return NotFound();
            }

            Task<List<LeagueWithSeasons>> leagueWithSeasonsTask = _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            Task<List<PlayerListItem>> playersTask = _leagueRepository.GetSeasonPlayersAsync(seasonId.Value, cancellationToken);
            Task<List<TeamModel>> teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId.Value, cancellationToken);

            await Task.WhenAll(leagueWithSeasonsTask, playersTask);

            return View(
                new PlayersViewModel
                {
                    LeagueSeasonChooser = new LeagueSeasonChooserViewModel(seasonId.Value, leagueWithSeasonsTask.Result, Url),
                    Players = playersTask.Result,
                    Teams = teamsTask.Result,
                });
        }

        public async Task<ActionResult> Teams(long? seasonId, CancellationToken cancellationToken)
        {
            if (seasonId is null)
            {
                return NotFound();
            }

            Task<List<LeagueWithSeasons>> leagueWithSeasonsTask = _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            Task<List<TeamModel>> teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId.Value, cancellationToken);

            await Task.WhenAll(leagueWithSeasonsTask, teamsTask);

            return View(
                new TeamsViewModel
                {
                    LeagueSeasonChooser = new LeagueSeasonChooserViewModel(seasonId.Value, leagueWithSeasonsTask.Result, Url),
                    Teams = teamsTask.Result,
                });
        }

        public async Task<ActionResult> Games(long? seasonId, CancellationToken cancellationToken)
        {
            if (seasonId is null)
            {
                return NotFound();
            }

            Task<List<LeagueWithSeasons>> leagueWithSeasonsTask = _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            Task<List<GameListItem>> gamesTask = _leagueRepository.GetSeasonGamesAsync(seasonId.Value, cancellationToken);
            Task<List<TeamModel>> teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId.Value, cancellationToken);

            await Task.WhenAll(leagueWithSeasonsTask, gamesTask, teamsTask);

            return View(
                new SeasonGamesViewModel
                {
                    LeagueSeasonChooser = new LeagueSeasonChooserViewModel(seasonId.Value, leagueWithSeasonsTask.Result, Url),
                    Games = gamesTask.Result,
                    Teams = teamsTask.Result,
                });
        }

        public async Task<ActionResult> Rounds(long? seasonId, CancellationToken cancellationToken)
        {
            if (seasonId is null)
            {
                return NotFound();
            }

            Task<List<LeagueWithSeasons>> leagueWithSeasonsTask = _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            Task<List<SeasonRound>> roundsTask = _leagueRepository.GetSeasonRoundsAsync(seasonId.Value, cancellationToken);

            await Task.WhenAll(leagueWithSeasonsTask, roundsTask);

            return View(
                new SeasonRoundListViewModel
                {
                    SeasonId = seasonId.Value,
                    LeagueSeasonChooser = new LeagueSeasonChooserViewModel(seasonId.Value, leagueWithSeasonsTask.Result, Url),
                    Rounds = roundsTask.Result,
                });
        }
    }
}
