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
        GameType? gameType,
        long? period,
        CancellationToken cancellationToken,
        int limit = 100,
        int offset = 0)
	{
		if (string.IsNullOrWhiteSpace(playerName))
			return BadRequest();

		if (gameType is not null && !Enum.IsDefined(gameType.Value))
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
				if (gameType is not null)
				{
					// Try to find the first available period that matches the specified game type.
					foreach (StatPeriod statPeriod in statPeriodList)
					{
						if (statPeriod.GameType == gameType.Value)
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
						gameType = selectedPeriod?.GameType,
						period = selectedPeriod?.StatPeriodId
					});
			}
			else if (selectedPeriod.Value.GameType != gameType)
			{
				return RedirectToAction(
					null,
					new
					{
						playerName,
						gameType = selectedPeriod?.GameType,
						period = selectedPeriod?.StatPeriodId
					});
			}

			GameCategory? gameCategory = selectedPeriod.Value.GameType.GetGameCategory();
			if (gameCategory is null)
			{
				return BadRequest();
			}

			// Different data/view based on game category.
			if (gameCategory.Value == GameCategory.TeamVersus)
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
				});
			}
			//else if (gameCategory == GameCategory.Solo) // TODO:
			//{
			//}
			//else if (gameCategory == GameCategory.Powerball) // TODO:
			//{
			//}
			else
			{
				_logger.LogError("Unsupported game category ({gameCategory}", gameCategory.Value);
				return RedirectToAction(null, new { playerName });
			}
		}
		else if (gameType is not null)
		{
			// Try to find the first available period that matches the specified game type.
			foreach (StatPeriod statPeriod in statPeriodList)
			{
				if (statPeriod.GameType == gameType.Value)
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
					gameType = selectedPeriod?.GameType,
					period = selectedPeriod?.StatPeriodId
				});
		}
		else
		{
			List<ParticipationRecord> participationRecordList = await _statsRepository.GetPlayerParticipationOverview(playerName, _options.PeriodCutoff, cancellationToken);

			// TODO: var recentMatches = 

			return View("Overview", new OverviewViewModel()
			{
				PlayerName = playerName,
				PlayerInfo = playerInfo,
				ParticipationRecordList = participationRecordList,
			});
		}


		async Task<List<TeamVersusPeriodStats>> GetPeriodStats(StatPeriod selectedPeriod)
		{
			StatPeriod? foreverStatPeriod = await _statsRepository.GetForeverStatPeriod(selectedPeriod.GameType, cancellationToken);
			List<StatPeriod> statPeriodList = foreverStatPeriod is not null
				? new(2) { selectedPeriod, foreverStatPeriod.Value }
				: new(1) { selectedPeriod };

			return await _statsRepository.GetTeamVersusPeriodStats(playerName, statPeriodList, cancellationToken);
		}
	}
}
