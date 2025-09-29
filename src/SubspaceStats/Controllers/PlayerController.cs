using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SubspaceStats.Models;
using SubspaceStats.Models.Player;
using SubspaceStats.Options;
using SubspaceStats.Services;

namespace SubspaceStats.Controllers;

public class PlayerController(
    IOptions<StatOptions> options,
    ILogger<PlayerController> logger,
    IStatsRepository statsRepository) : Controller
{
    private readonly StatOptions _options = options.Value;
    private readonly ILogger<PlayerController> _logger = logger;
    private readonly IStatsRepository _statsRepository = statsRepository;

    public async Task<IActionResult> Index(
        string playerName,
        [Bind(Prefix ="gameType")]long? gameTypeId,
        long? period,
        CancellationToken cancellationToken,
        int limit = 100,
        int offset = 0)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            return BadRequest();

        limit = Math.Clamp(limit, 1, 200);

        (PlayerInfo PlayerInfo, List<StatPeriod> StatPeriodList)? info = await _statsRepository.GetPlayerInfoAndStatPeriods(playerName, _options.PeriodCutoff, cancellationToken);
        if (info is null)
        {
            return NotFound();
        }

        (PlayerInfo playerInfo, List<StatPeriod> statPeriodList) = info.Value;

        StatPeriod? selectedPeriod = null;
        if (period is not null)
        {
            // Search for the specified period.
            foreach (StatPeriod statPeriod in statPeriodList)
            {
                if (statPeriod.StatPeriodId == period.Value)
                {
                    selectedPeriod = statPeriod;
                    break;
                }
            }

            if (selectedPeriod is null)
            {
                // The specified period is invalid.
                if (gameTypeId is not null)
                {
                    // Try to find the first available period that matches the specified game type.
                    foreach (StatPeriod statPeriod in statPeriodList)
                    {
                        if (statPeriod.GameTypeId == gameTypeId.Value)
                        {
                            selectedPeriod = statPeriod;
                            break;
                        }
                    }
                }

                return RedirectToAction(
                    null,
                    new
                    {
                        playerName,
                        gameType = selectedPeriod?.GameTypeId,
                        period = selectedPeriod?.StatPeriodId
                    });
            }
            else if (selectedPeriod.Value.GameTypeId != gameTypeId)
            {
                return RedirectToAction(
                    null,
                    new
                    {
                        playerName,
                        gameType = selectedPeriod?.GameTypeId,
                        period = selectedPeriod?.StatPeriodId
                    });
            }

            GameType? gameType = await _statsRepository.GetGameTypeAsync(selectedPeriod.Value.GameTypeId, cancellationToken);
            if (gameType is null)
            {
                return BadRequest();
            }

            // Different data/view based on game category.
            if (gameType.GameMode == GameMode.TeamVersus)
            {
                var periodStatsTask = GetPeriodStats(selectedPeriod.Value);
                var gameStatsTask = _statsRepository.GetTeamVersusGameStats(playerName, selectedPeriod.Value.StatPeriodId, limit + 1, offset, cancellationToken);
                var shipStatsTask = _statsRepository.GetTeamVersusShipStats(playerName, selectedPeriod.Value.StatPeriodId, cancellationToken);
                var killStatsTask = _statsRepository.GetTeamVersusKillStats(playerName, selectedPeriod.Value.StatPeriodId, _options.PlayerDetails.KillStatsLimit, cancellationToken);

                await Task.WhenAll(periodStatsTask, gameStatsTask, shipStatsTask, killStatsTask);

                var gameStatsList = gameStatsTask.Result;
                bool hasMore = false;
                if (gameStatsList.Count == limit + 1)
                {
                    gameStatsList.RemoveAt(gameStatsList.Count - 1);
                    hasMore = true;
                }

                return View("TeamVersusHistory", new TeamVersusHistoryViewModel()
                {
                    PlayerName = playerName,
                    PlayerInfo = playerInfo,
                    PeriodList = statPeriodList,
                    SelectedPeriod = selectedPeriod.Value,
                    PeriodStatsList = periodStatsTask.Result,
                    GameStatsList = gameStatsList,
                    GameStatsPaging = new()
                    {
                        Limit = limit,
                        Offset = offset,
                        HasMore = hasMore,
                    },
                    ShipStatsList = shipStatsTask.Result,
                    KillStatsList = killStatsTask.Result,
                    GameTypes = await _statsRepository.GetGameTypesAsync(cancellationToken),
                });
            }
            //else if (gameType.GameMode == GameMode.Solo) // TODO:
            //{
            //}
            //else if (gameType.GameMode == GameMode.Powerball) // TODO:
            //{
            //}
            else
            {
                _logger.LogError("Unsupported game mode ({GameMode}).", gameType.GameMode);
                return RedirectToAction(null, new { playerName });
            }
        }
        else if (gameTypeId is not null)
        {
            // Try to find the first available period that matches the specified game type.
            foreach (StatPeriod statPeriod in statPeriodList)
            {
                if (statPeriod.GameTypeId == gameTypeId.Value)
                {
                    selectedPeriod = statPeriod;
                    break;
                }
            }

            return RedirectToAction(
                null,
                new
                {
                    playerName,
                    gameType = selectedPeriod?.GameTypeId,
                    period = selectedPeriod?.StatPeriodId
                });
        }
        else
        {
            var participationRecordsTask = _statsRepository.GetPlayerParticipationOverview(playerName, _options.PeriodCutoff, cancellationToken);
            var gameTypesTask = _statsRepository.GetGameTypesAsync(cancellationToken);
            // TODO: var recentMatches = 

            await Task.WhenAll(participationRecordsTask, gameTypesTask);

            return View("Overview", new OverviewViewModel()
            {
                PlayerName = playerName,
                PlayerInfo = playerInfo,
                ParticipationRecordList = participationRecordsTask.Result,
                GameTypes = gameTypesTask.Result,
            });
        }


        async Task<List<TeamVersusPeriodStats>> GetPeriodStats(StatPeriod selectedPeriod)
        {
            StatPeriod? foreverStatPeriod = await _statsRepository.GetForeverStatPeriod(selectedPeriod.GameTypeId, cancellationToken);
            List<StatPeriod> statPeriodList = foreverStatPeriod is not null
                ? new(2) { selectedPeriod, foreverStatPeriod.Value }
                : new(1) { selectedPeriod };

            return await _statsRepository.GetTeamVersusPeriodStats(playerName, statPeriodList, cancellationToken);
        }
    }
}
