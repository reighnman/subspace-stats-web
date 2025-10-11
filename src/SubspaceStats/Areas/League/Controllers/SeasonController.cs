using Microsoft.AspNetCore.Mvc;
using SubspaceStats.Areas.League.Models;
using SubspaceStats.Areas.League.Models.Franchise;
using SubspaceStats.Areas.League.Models.League;
using SubspaceStats.Areas.League.Models.Season;
using SubspaceStats.Areas.League.Models.SeasonGame;
using SubspaceStats.Areas.League.Models.Team;
using SubspaceStats.Services;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Controllers
{
    [Area("League")]
    public class SeasonController(
        ILeagueRepository leagueRepository,
        IStatsRepository statsRepository) : Controller
    {
        private readonly ILeagueRepository _leagueRepository = leagueRepository;
        private readonly IStatsRepository _statsRepository = statsRepository;

        // GET Season/{seasonId}
        public async Task<ActionResult> IndexAsync(long? seasonId, CancellationToken cancellationToken)
        {
            if (seasonId is null)
            {
                return NotFound();
            }

            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(seasonId.Value, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            List<LeagueNavItem> leaguesWithSeasons = await _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            LeagueSeasonChooserViewModel seasonChooser = new(seasonId.Value, leaguesWithSeasons, Url);
            LeagueNavItem? currentLeague = seasonChooser.SelectedLeague;
            SeasonNavItem? currentSeason = seasonChooser.SelectedSeason;
            if (currentLeague is null || currentSeason is null)
            {
                return NotFound();
            }

            Task<List<ScheduledGame>> scheduledGamesTask = _leagueRepository.GetScheduledGamesAsync(seasonId.Value, cancellationToken);
            Task<List<TeamStanding>> standingsTask = _leagueRepository.GetSeasonStandingsAsync(seasonId.Value, cancellationToken);
            Task<List<GameRecord>> completedGamesTask = _leagueRepository.GetCompletedGamesAsync(seasonId.Value, cancellationToken);

            await Task.WhenAll(scheduledGamesTask, standingsTask, completedGamesTask);

            return View(
                new OverviewViewModel()
                {
                    SeasonDetails = seasonDetails,
                    League = currentLeague,
                    Season = currentSeason,
                    LeagueSeasonChooser = seasonChooser,
                    ScheduledGames = scheduledGamesTask.Result,
                    Standings = standingsTask.Result,
                    CompletedGames = completedGamesTask.Result,
                });
        }

        // GET Season/{seasonId}/Rosters
        public async Task<ActionResult> Rosters(long seasonId, CancellationToken cancellationToken)
        {
            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(seasonId, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            List<LeagueNavItem> leaguesWithSeasons = await _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            LeagueSeasonChooserViewModel seasonChooser = new(seasonId, leaguesWithSeasons, Url);
            LeagueNavItem? currentLeague = seasonChooser.SelectedLeague;
            SeasonNavItem? currentSeason = seasonChooser.SelectedSeason;
            if (currentLeague is null || currentSeason is null)
            {
                return NotFound();
            }

            return View(
                new RostersViewModel()
                {
                    SeasonDetails = seasonDetails,
                    League = currentLeague,
                    Season = currentSeason,
                    LeagueSeasonChooser = seasonChooser,
                    Rosters = await _leagueRepository.GetSeasonRostersAsync(seasonId, cancellationToken),
                });
        }

        // GET Season/{seasonId}/Details
        public async Task<ActionResult> Details(long seasonId, CancellationToken cancellationToken)
        {
            SeasonDetails? details = await _leagueRepository.GetSeasonDetailsAsync(seasonId, cancellationToken);
            if (details is null)
            {
                return NotFound();
            }

            List<LeagueNavItem> leaguesWithSeasons = await _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            LeagueSeasonChooserViewModel seasonChooser = new(seasonId, leaguesWithSeasons, Url);
            LeagueNavItem? currentLeague = seasonChooser.SelectedLeague;
            SeasonNavItem? currentSeason = seasonChooser.SelectedSeason;
            if (currentLeague is null || currentSeason is null)
            {
                return NotFound();
            }

            return View(
                new DetailsViewModel
                {
                    SeasonDetails = details,
                    League = currentLeague,
                    Season = currentSeason,
                    LeagueSeasonChooser = seasonChooser,
                    GameTypes = await _statsRepository.GetGameTypesAsync(cancellationToken),
                });
        }

        // POST Season/{seasonId}/Start
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Start(long seasonId, DateOnly? startDate, CancellationToken cancellationToken)
        {
            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(seasonId, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            if (seasonDetails.StartDate is not null)
            {
                return Conflict();
            }

            await _leagueRepository.StartSeasonAsync(seasonId, startDate, cancellationToken);
            return RedirectToAction("Details");
        }

        // POST Season/{seasonId}/End
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> End(long seasonId, [Required]DateOnly? endDate, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(seasonId, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            if (seasonDetails.StartDate is null || seasonDetails.EndDate is not null)
            {
                return Conflict();
            }

            await _leagueRepository.UpdateSeasonEndDateAsync(seasonId, endDate, cancellationToken);
            return RedirectToAction("Details");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UndoEnd(long seasonId, CancellationToken cancellationToken)
        {
            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(seasonId, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            if (seasonDetails.StartDate is null || seasonDetails.EndDate is null)
            {
                return Conflict();
            }

            await _leagueRepository.UpdateSeasonEndDateAsync(seasonId, null, cancellationToken);
            return RedirectToAction("Details");
        }

        // GET Season/{seasonId}/Copy
        public async Task<ActionResult> Copy(long seasonId, CancellationToken cancellationToken)
        {
            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(seasonId, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            Task<List<PlayerListItem>> playersTask = _leagueRepository.GetSeasonPlayersAsync(seasonId, cancellationToken);
            Task<OrderedDictionary<long, TeamModel>> teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId, cancellationToken);
            Task<List<GameModel>> gamesTask = _leagueRepository.GetSeasonGamesAsync(seasonId, cancellationToken);
            Task<OrderedDictionary<int, SeasonRound>> roundsTask = _leagueRepository.GetSeasonRoundsAsync(seasonId, cancellationToken);
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
                    IncludePlayers = true,
                    IncludeTeams = true,
                    IncludeGames = true,
                    IncludeRounds = true,
                });
        }

        // POST Season/{seasonId}/Copy
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
                Task<OrderedDictionary<long, TeamModel>> teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId, cancellationToken);
                Task<List<GameModel>> gamesTask = _leagueRepository.GetSeasonGamesAsync(seasonId, cancellationToken);
                Task<OrderedDictionary<int, SeasonRound>> roundsTask = _leagueRepository.GetSeasonRoundsAsync(seasonId, cancellationToken);
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

        // GET Season/{seasonId}/Players
        public async Task<ActionResult> Players(long? seasonId, CancellationToken cancellationToken)
        {
            if (seasonId is null)
            {
                return NotFound();
            }

            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(seasonId.Value, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            List<LeagueNavItem> leaguesWithSeasons = await _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            LeagueSeasonChooserViewModel seasonChooser = new(seasonId.Value, leaguesWithSeasons, Url);
            LeagueNavItem? currentLeague = seasonChooser.SelectedLeague;
            SeasonNavItem? currentSeason = seasonChooser.SelectedSeason;
            if (currentLeague is null || currentSeason is null)
            {
                return NotFound();
            }

            Task<List<PlayerListItem>> playersTask = _leagueRepository.GetSeasonPlayersAsync(seasonId.Value, cancellationToken);
            Task<OrderedDictionary<long, TeamModel>> teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId.Value, cancellationToken);

            await Task.WhenAll(playersTask, teamsTask);

            return View(
                new PlayersViewModel
                {
                    SeasonDetails = seasonDetails,
                    League = currentLeague,
                    Season = currentSeason,
                    LeagueSeasonChooser = seasonChooser,
                    Players = playersTask.Result,
                    Teams = teamsTask.Result,
                });
        }

        // GET Season/{seasonId}/Teams
        public async Task<ActionResult> Teams(long? seasonId, CancellationToken cancellationToken)
        {
            if (seasonId is null)
            {
                return NotFound();
            }

            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(seasonId.Value, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            List<LeagueNavItem> leaguesWithSeasons = await _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            LeagueSeasonChooserViewModel seasonChooser = new(seasonId.Value, leaguesWithSeasons, Url);
            LeagueNavItem? currentLeague = seasonChooser.SelectedLeague;
            SeasonNavItem? currentSeason = seasonChooser.SelectedSeason;
            if (currentLeague is null || currentSeason is null)
            {
                return NotFound();
            }

            Task<OrderedDictionary<long, TeamModel>> teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId.Value, cancellationToken);
            Task<OrderedDictionary<long, FranchiseModel>> franchiseTask = _leagueRepository.GetFranchisesAsync(cancellationToken);

            await Task.WhenAll(teamsTask, franchiseTask);

            return View(
                new TeamsViewModel
                {
                    SeasonDetails = seasonDetails,
                    League = currentLeague,
                    Season = currentSeason,
                    LeagueSeasonChooser = seasonChooser,
                    Teams = teamsTask.Result,
                    Franchises = franchiseTask.Result,
                });
        }

        // GET Season/{seasonId}/Games
        public async Task<ActionResult> Games(long? seasonId, CancellationToken cancellationToken)
        {
            if (seasonId is null)
            {
                return NotFound();
            }

            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(seasonId.Value, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            var season = await _leagueRepository.GetSeasonAsync(seasonId.Value, cancellationToken);
            if (season is null)
            {
                return NotFound();
            }

            var league = await _leagueRepository.GetLeagueAsync(season.LeagueId, cancellationToken);
            if (league is null)
            {
                return NotFound();
            }

            List<LeagueNavItem> leaguesWithSeasons = await _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            LeagueSeasonChooserViewModel seasonChooser = new(seasonId.Value, leaguesWithSeasons, Url);
            LeagueNavItem? currentLeague = seasonChooser.SelectedLeague;
            SeasonNavItem? currentSeason = seasonChooser.SelectedSeason;
            if (currentLeague is null || currentSeason is null)
            {
                return NotFound();
            }

            Task<List<GameModel>> gamesTask = _leagueRepository.GetSeasonGamesAsync(seasonId.Value, cancellationToken);
            Task<OrderedDictionary<long, TeamModel>> teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId.Value, cancellationToken);
            Task<OrderedDictionary<int, SeasonRound>> roundsTask = _leagueRepository.GetSeasonRoundsAsync(seasonId.Value, cancellationToken);

            await Task.WhenAll(gamesTask, teamsTask, roundsTask);

            return View(
                new GamesViewModel
                {
                    SeasonDetails = seasonDetails,
                    LeagueNav = currentLeague,
                    SeasonNav = currentSeason,
                    League = league,
                    Season = season,
                    LeagueSeasonChooser = seasonChooser,
                    Games = gamesTask.Result,
                    Teams = teamsTask.Result,
                    Rounds = roundsTask.Result,
                });
        }

        // GET Season/{seasonId}/Rounds
        public async Task<ActionResult> Rounds(long? seasonId, CancellationToken cancellationToken)
        {
            if (seasonId is null)
            {
                return NotFound();
            }

            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(seasonId.Value, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            List<LeagueNavItem> leaguesWithSeasons = await _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            LeagueSeasonChooserViewModel seasonChooser = new(seasonId.Value, leaguesWithSeasons, Url);
            LeagueNavItem? currentLeague = seasonChooser.SelectedLeague;
            SeasonNavItem? currentSeason = seasonChooser.SelectedSeason;
            if (currentLeague is null || currentSeason is null)
            {
                return NotFound();
            }

            return View(
                new RoundsViewModel
                {
                    SeasonDetails = seasonDetails,
                    League = currentLeague,
                    Season = currentSeason,
                    LeagueSeasonChooser = seasonChooser,
                    Rounds = await _leagueRepository.GetSeasonRoundsAsync(seasonId.Value, cancellationToken),
                });
        }

        // GET League/Season/Create
        public async Task<ActionResult> Create(long? leagueId, CancellationToken cancellationToken)
        {
            List<LeagueModel> leagues = await _leagueRepository.GetLeagueListAsync(cancellationToken);
            LeagueModel? selectedLeague = null;
            if (leagueId is not null)
            {
                selectedLeague = leagues.Find(l => l.Id == leagueId.Value);
            }

            return View(
                new CreateSeasonViewModel
                {
                    Model = new CreateSeasonModel
                    {
                        SeasonName = "",
                        LeagueId = selectedLeague?.Id
                    },
                    Leagues = leagues,
                });
        }

        // POST League/Season/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateSeasonModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    new CreateSeasonViewModel
                    {
                        Model = model,
                        Leagues = await _leagueRepository.GetLeagueListAsync(cancellationToken),
                    });
            }

            List<LeagueModel> leagues = await _leagueRepository.GetLeagueListAsync(cancellationToken);
            LeagueModel? selectedLeague = null;
            if (model.LeagueId is not null)
            {
                selectedLeague = leagues.Find(l => l.Id == model.LeagueId.Value);
            }

            if (selectedLeague is null)
            {
                ModelState.AddModelError($"Model.{model.LeagueId}", "League not found.");

                return View(
                    new CreateSeasonViewModel
                    {
                        Model = model,
                        Leagues = await _leagueRepository.GetLeagueListAsync(cancellationToken),
                    });
            }

            long seasonId = await _leagueRepository.InsertSeasonAsync(model.SeasonName, selectedLeague.Id, cancellationToken);
            return RedirectToAction("Details", "Season", new { seasonId });
        }

        // GET Season/{seasonId}/Edit
        public async Task<ActionResult> Edit(long seasonId, CancellationToken cancellationToken)
        {
            SeasonModel? season = await _leagueRepository.GetSeasonAsync(seasonId, cancellationToken);
            if (season is null)
            {
                return NotFound();
            }

            LeagueModel? league = await _leagueRepository.GetLeagueAsync(season.LeagueId, cancellationToken);
            if (league is null)
            {
                return NotFound();
            }

            return View(
                new SeasonViewModel
                {
                    Model = season,
                    League = league,
                });
        }

        // POST Season/{seasonId}/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long seasonId, SeasonModel model, CancellationToken cancellationToken)
        {
            SeasonModel? season = await _leagueRepository.GetSeasonAsync(seasonId, cancellationToken);
            if (season is null)
            {
                return NotFound();
            }

            LeagueModel? league = await _leagueRepository.GetLeagueAsync(season.LeagueId, cancellationToken);
            if (league is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(
                    new SeasonViewModel
                    {
                        Model = season,
                        League = league,
                    });
            }

            await _leagueRepository.UpdateSeasonAsync(seasonId, model.SeasonName, cancellationToken);
            return RedirectToAction("Details");
        }

        // GET Season/{seasonId}/Delete
        public async Task<ActionResult> Delete(long seasonId, CancellationToken cancellationToken)
        {
            SeasonModel? season = await _leagueRepository.GetSeasonAsync(seasonId, cancellationToken);
            if (season is null)
            {
                return NotFound();
            }

            LeagueModel? league = await _leagueRepository.GetLeagueAsync(season.LeagueId, cancellationToken);
            if (league is null)
            {
                return NotFound();
            }

            return View(
                new SeasonViewModel
                {
                    Model = season,
                    League = league,
                });
        }

        // POST Season/{seasonId}/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirm(long seasonId, CancellationToken cancellationToken)
        {
            SeasonModel? season = await _leagueRepository.GetSeasonAsync(seasonId, cancellationToken);
            if (season is null)
            {
                return NotFound();
            }

            await _leagueRepository.DeleteSeasonAsync(seasonId, cancellationToken);
            return RedirectToAction("Details", "League", new { leagueId = season.LeagueId });
        }
    }
}
