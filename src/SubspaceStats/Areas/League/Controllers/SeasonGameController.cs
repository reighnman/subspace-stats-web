using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubspaceStats.Areas.League.Authorization;
using SubspaceStats.Areas.League.Models.League;
using SubspaceStats.Areas.League.Models.Season;
using SubspaceStats.Areas.League.Models.Season.Game;
using SubspaceStats.Areas.League.Models.SeasonGame;
using SubspaceStats.Services;

namespace SubspaceStats.Areas.League.Controllers
{
    [Area("League")]
    public class SeasonGameController(
        IAuthorizationService authorizationService,
        ILeagueRepository leagueRepository) : Controller
    {
        private readonly IAuthorizationService _authorizationService = authorizationService;
        private readonly ILeagueRepository _leagueRepository = leagueRepository;

        // GET League/Season/{seasonId}/Games/Create
        public async Task<IActionResult> Create(long seasonId, CancellationToken cancellationToken)
        {
            // Validate seasonId
            SeasonModel? season = await _leagueRepository.GetSeasonAsync(seasonId, cancellationToken);
            if (season is null)
            {
                return NotFound();
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, season, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return Forbid();
                }
                else
                {
                    return Challenge();
                }
            }

            LeagueModel? league = await _leagueRepository.GetLeagueAsync(season.LeagueId, cancellationToken);
            if (league is null)
            {
                return NotFound();
            }

            var gamesTask = _leagueRepository.GetSeasonGamesAsync(seasonId, cancellationToken);
            var teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId, cancellationToken);
            var roundsTask = _leagueRepository.GetSeasonRoundsAsync(seasonId, cancellationToken);

            await Task.WhenAll(teamsTask, roundsTask);

            // Start the round as the current max round number, or 1 if there is no max.
            int roundNumber = 1;
            foreach (GameModel game in gamesTask.Result)
            {
                if (game.RoundNumber > roundNumber)
                    roundNumber = game.RoundNumber;
            }

            // Create the teams and set their freq
            List<GameTeamModel> teams = new(league.MinTeamsPerGame);
            for (int i = 0; i < league.MinTeamsPerGame; i++)
            {
                teams.Add(
                    new GameTeamModel()
                    {
                        Freq = (short)(league.FreqStart + (i * league.FreqIncrement)),
                    });
            }

            return View(
                new GameViewModel
                {
                    Game = new GameModel
                    {
                        SeasonId = seasonId,
                        RoundNumber = roundNumber,
                        Status = Models.GameStatus.Pending,
                        Teams = teams,
                    },
                    Season = season,
                    League = league,
                    AutoAssignFreqs = true,
                    IsReadOnly = false,
                    Teams = teamsTask.Result,
                    Rounds = roundsTask.Result,
                });
        }

        // POST League/Season/{seasonId}/Games/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(long seasonId, GameModel game, bool autoAssignFreqs, CancellationToken cancellationToken)
        {
            // Validate seasonId
            SeasonModel? season = await _leagueRepository.GetSeasonAsync(seasonId, cancellationToken);
            if (season is null)
            {
                return NotFound();
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, season, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return Forbid();
                }
                else
                {
                    return Challenge();
                }
            }

            LeagueModel? league = await _leagueRepository.GetLeagueAsync(season.LeagueId, cancellationToken);
            if (league is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId, cancellationToken);
                var roundsTask = _leagueRepository.GetSeasonRoundsAsync(seasonId, cancellationToken);

                await Task.WhenAll(teamsTask, roundsTask);

                return View(
                    new GameViewModel
                    {
                        Game = game,
                        Season = season,
                        League = league,
                        AutoAssignFreqs = autoAssignFreqs,
                        IsReadOnly = false,
                        Teams = teamsTask.Result,
                        Rounds = roundsTask.Result,
                    });
            }

            long seasonGameId = await _leagueRepository.InsertSeasonGameAsync(seasonId, game, cancellationToken);
            
            // pass the seasonGameId in the query string so that the page can highlight the record if it wants to
            return RedirectToAction("Games", "Season", new { seasonId, seasonGameId });
        }

        // GET League/Season/{seasonId}/Games/{seasonGameId}/Edit
        public async Task<IActionResult> Edit(long seasonId, long seasonGameId, CancellationToken cancellationToken)
        {
            // Validate seasonId
            SeasonModel? season = await _leagueRepository.GetSeasonAsync(seasonId, cancellationToken);
            if (season is null)
            {
                return NotFound();
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, season, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return Forbid();
                }
                else
                {
                    return Challenge();
                }
            }

            // Validate seasonGameId
            GameModel? game = await _leagueRepository.GetSeasonGameAsync(seasonGameId, cancellationToken);
            if (game is null || game.SeasonId != seasonId)
            {
                return NotFound();
            }

            LeagueModel? league = await _leagueRepository.GetLeagueAsync(season.LeagueId, cancellationToken);
            if (league is null)
            {
                return NotFound();
            }

            var teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId, cancellationToken);
            var roundsTask = _leagueRepository.GetSeasonRoundsAsync(seasonId, cancellationToken);

            await Task.WhenAll(teamsTask, roundsTask);

            return View(
                new GameViewModel
                {
                    Game = game,
                    Season = season,
                    League = league,
                    AutoAssignFreqs = false,
                    IsReadOnly = false,
                    Teams = teamsTask.Result,
                    Rounds = roundsTask.Result,
                });
        }

        // POST League/Season/{seasonId}/Games/{seasonGameId}/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long seasonId, long seasonGameId, GameModel game, bool autoAssignFreqs, CancellationToken cancellationToken)
        {
            if (seasonId != game.SeasonId || seasonGameId != game.SeasonGameId)
            {
                return NotFound();
            }

            // Validate seasonId
            SeasonModel? season = await _leagueRepository.GetSeasonAsync(seasonId, cancellationToken);
            if (season is null)
            {
                return NotFound();
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, season, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return Forbid();
                }
                else
                {
                    return Challenge();
                }
            }

            // Validate seasonGameId
            GameModel? oldGame = await _leagueRepository.GetSeasonGameAsync(seasonGameId, cancellationToken);
            if (oldGame is null || oldGame.SeasonId != seasonId)
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
                var teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId, cancellationToken);
                var roundsTask = _leagueRepository.GetSeasonRoundsAsync(seasonId, cancellationToken);

                await Task.WhenAll(teamsTask, roundsTask);

                return View(
                    new GameViewModel
                    {
                        Game = game,
                        Season = season,
                        League = league,
                        AutoAssignFreqs = autoAssignFreqs,
                        IsReadOnly = false,
                        Teams = teamsTask.Result,
                        Rounds = roundsTask.Result,
                    });
            }

            await _leagueRepository.UpdateSeasonGameAsync(game, cancellationToken);

            // pass the seasonGameId in the query string so that the page can highlight the record if it wants to
            return RedirectToAction("Games", "Season", new { seasonId = season.SeasonId, seasonGameId });
        }

        // GET League/Season/{seasonId}/Games/{seasonGameId}/Delete
        public async Task<IActionResult> Delete(long seasonId, long seasonGameId, CancellationToken cancellationToken)
        {
            // Validate seasonId
            SeasonModel? season = await _leagueRepository.GetSeasonAsync(seasonId, cancellationToken);
            if (season is null)
            {
                return NotFound();
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, season, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return Forbid();
                }
                else
                {
                    return Challenge();
                }
            }

            // Validate seasonGameId
            GameModel? game = await _leagueRepository.GetSeasonGameAsync(seasonGameId, cancellationToken);
            if (game is null || game.SeasonId != seasonId)
            {
                return NotFound();
            }

            LeagueModel? league = await _leagueRepository.GetLeagueAsync(season.LeagueId, cancellationToken);
            if (league is null)
            {
                return NotFound();
            }

            var teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId, cancellationToken);
            var roundsTask = _leagueRepository.GetSeasonRoundsAsync(seasonId, cancellationToken);

            await Task.WhenAll(teamsTask, roundsTask);

            return View(
                new GameViewModel
                {
                    Game = game,
                    Season = season,
                    League = league,
                    AutoAssignFreqs = false,
                    IsReadOnly = true,
                    Teams = teamsTask.Result,
                    Rounds = roundsTask.Result,
                });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(long seasonId, long seasonGameId, CancellationToken cancellationToken)
        {
            // Validate seasonId
            SeasonModel? season = await _leagueRepository.GetSeasonAsync(seasonId, cancellationToken);
            if (season is null)
            {
                return NotFound();
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, season, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return Forbid();
                }
                else
                {
                    return Challenge();
                }
            }

            // Validate seasonGameId
            GameModel? game = await _leagueRepository.GetSeasonGameAsync(seasonGameId, cancellationToken);
            if (game is null || game.SeasonId != seasonId)
            {
                return NotFound();
            }

            await _leagueRepository.DeleteSeasonGame(seasonGameId, cancellationToken);
            return RedirectToAction("Games", "Season", new { seasonId });
        }

        // GET Season/{seasonId}/Games/AddFullRound
        public async Task<IActionResult> AddFullRound(long seasonId, CancellationToken cancellationToken)
        {
            // Validate seasonId
            SeasonModel? season = await _leagueRepository.GetSeasonAsync(seasonId, cancellationToken);
            if (season is null)
            {
                return NotFound();
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, season, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return Forbid();
                }
                else
                {
                    return Challenge();
                }
            }

            LeagueModel? league = await _leagueRepository.GetLeagueAsync(season.LeagueId, cancellationToken);
            if (league is null)
            {
                return NotFound();
            }

            return View(
                new AddFullRoundViewModel
                {
                    Season = season,
                    League = league,
                    Mode = FullRoundOf.Combinations,
                });
        }

        // POST Season/{seasonId}/Games/AddFullRound
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFullRound(long seasonId, [Bind("Mode")] AddFullRoundViewModel model, CancellationToken cancellationToken)
        {
            SeasonModel? season = await _leagueRepository.GetSeasonAsync(seasonId, cancellationToken);
            if (season is null)
            {
                return NotFound();
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, season, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return Forbid();
                }
                else
                {
                    return Challenge();
                }
            }

            LeagueModel? league = await _leagueRepository.GetLeagueAsync(season.LeagueId, cancellationToken);
            if (league is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.Season = season;
                model.League = league;
                return View(model);
            }

            await _leagueRepository.InsertSeasonGamesForRoundWith2TeamsAsync(seasonId, model.Mode, cancellationToken);
            return RedirectToAction("Games", "Season", new { seasonId });
        }
    }
}
