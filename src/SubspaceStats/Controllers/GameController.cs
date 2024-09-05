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
            return NotFound();

        GameCategory? gameCategory = game.GameType.GetGameCategory();
        if (gameCategory is null)
        {
            _logger.LogError("Unknown GameCategory for GameType ({gameType}).", game.GameType);
            return NotFound();
        }

        switch (gameCategory.Value)
        {
            case GameCategory.TeamVersus:
                return View("TeamVersusGame", game);

            case GameCategory.Solo:
                //TODO: return View("SoloGame", game);
                goto default;

            case GameCategory.Powerball:
                //TODO: return View("PowerballGame", game);
                goto default;

            default:
                _logger.LogError("Unsupported GameCategory ({gameCategory}).", gameCategory.Value);
                return NoContent();
        }
    }
}
