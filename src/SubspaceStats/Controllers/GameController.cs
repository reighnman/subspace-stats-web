using Microsoft.AspNetCore.Mvc;
using SubspaceStats.Models;
using SubspaceStats.Models.GameDetails;
using SubspaceStats.Services;

namespace SubspaceStats.Controllers;

public class GameController(
    ILogger<GameController> logger,
    IStatsRepository statsRepository) : Controller
{
    private readonly ILogger<GameController> _logger = logger;
    private readonly IStatsRepository _statsRepository = statsRepository;

    public async Task<IActionResult> Index(long id, CancellationToken cancellationToken)
    {
        Game? game = await _statsRepository.GetGameAsync(id, cancellationToken);
        if (game is null)
        {
            return NotFound();
        }

        GameType? gameType = await _statsRepository.GetGameTypeAsync(game.GameTypeId, cancellationToken);
        if (gameType is null)
        {
            return UnprocessableEntity();
        }

        switch (gameType.GameMode)
        {
            case GameMode.TeamVersus:
                return View(
                    "TeamVersusGame", 
                    new GameDetailsViewModel
                    {
                        Game = game,
                        GameType = gameType
                    });

            case GameMode.OneVersusOne:
                //TODO: return View("SoloGame", game);
                goto default;

            case GameMode.Powerball:
                //TODO: return View("PowerballGame", game);
                goto default;

            default:
                _logger.LogError("Unsupported GameMode ({GameMode}).", gameType.GameMode);
                return NoContent();
        }
    }
}
