using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SubspaceStats.Models;
using SubspaceStats.Models.Leaderboard;
using SubspaceStats.Options;
using SubspaceStats.Services;

namespace SubspaceStats.Controllers
{
    public class LeaderboardController : Controller
    {
		private readonly StatOptions _options;
        private readonly IStatsRepository _statsRepository;

        public LeaderboardController(
			IOptions<StatOptions> options, 
            IStatsRepository statsRepository)
        {
			_options = options.Value;
            _statsRepository = statsRepository;
        }

		public async Task<IActionResult> Index(
            GameType gameType, 
            CancellationToken cancellationToken, 
            long? period = null, 
            TeamVersusLeaderboardSort sort = TeamVersusLeaderboardSort.Rating, // TODO: implement sorting
            int limit = 100, 
            int offset = 0)
        {
            limit = Math.Clamp(limit, 1, 200);

            List<StatPeriod> periods = await _statsRepository.GetStatPeriods(gameType, StatPeriodType.Monthly, 12, 0, cancellationToken);

            StatPeriod? selectedPeriod = null;
            StatPeriod? priorPeriod = null;

            if (periods.Count > 0)
            {
                if (period is not null)
                {
                    int selectedIndex = periods.FindIndex(rp => rp.StatPeriodId == period);
                    if (selectedIndex == -1)
					{
						return RedirectToAction((string)RouteData.Values["action"]!, new { gameType });
					}
					
                    selectedPeriod = periods[selectedIndex];
                    if (selectedIndex + 1 < periods.Count)
                    {
                        priorPeriod = periods[selectedIndex + 1];
                    }
                }

                if (selectedPeriod is null)
                {
                    selectedPeriod = periods[0];
                    if (periods.Count > 1)
                    {
						priorPeriod = periods[1];
					}
                }
            }

            List<TopRatingRecord>? topRatingList = null;
			List<TopAvgRatingRecord>? topAvgRatingList = null;
            List<TopKillsPerMinuteRecord>? topKillsPerMinuteList = null;
			List<TopRatingRecord>? topRatingLastMonthList = null;
			List<TeamVersusLeaderboardStats>? statsList = null;

			if (selectedPeriod is not null)
            {
				var topRatingTask = _statsRepository.GetTopPlayersByRating(selectedPeriod.Value.StatPeriodId, 5, cancellationToken);
				var topAvgRatingTask = _statsRepository.GetTopTeamVersusPlayersByAvgRating(selectedPeriod.Value.StatPeriodId, 5, _options.Top.AvgRating.MinGamesPlayed, cancellationToken);
				var topKillsPerMinuteTask = _statsRepository.GetTopTeamVersusPlayersByKillsPerMinute(selectedPeriod.Value.StatPeriodId, 5, _options.Top.KillsPerMinute.MinGamesPlayed, cancellationToken);

				Task<List<TopRatingRecord>?> topRatingLastMonthTask;
				if (priorPeriod is not null)
				{
					topRatingLastMonthTask = _statsRepository.GetTopPlayersByRating(priorPeriod.Value.StatPeriodId, 5, cancellationToken)!;
				}
				else
				{
					topRatingLastMonthTask = Task.FromResult<List<TopRatingRecord>?>(null);
				}

				var statListTask = _statsRepository.GetTeamVersusLeaderboardAsync(selectedPeriod.Value.StatPeriodId, limit + 1, offset, cancellationToken);

				await Task.WhenAll(topRatingTask, topAvgRatingTask, topKillsPerMinuteTask, statListTask, topRatingLastMonthTask);

				topRatingList = topRatingTask.Result;
				topAvgRatingList = topAvgRatingTask.Result;
				topKillsPerMinuteList = topKillsPerMinuteTask.Result;
				statsList = statListTask.Result;
				topRatingLastMonthList = topRatingLastMonthTask.Result;
			}

            bool hasMore = false;
            if (statsList is not null && statsList.Count == limit + 1)
            {
                statsList.RemoveAt(statsList.Count - 1);
                hasMore = true;
            }

            return View("TeamVersusLeaderboard", new TeamVersusLeaderboardViewModel()
            {
                GameType = gameType,
                Periods = periods,
                SelectedPeriod = selectedPeriod,
                PriorPeriod = priorPeriod,
                TopRatingList = topRatingList,
                TopAvgRatingList = topAvgRatingList,
                TopKillsPerMinuteList = topKillsPerMinuteList,
                TopRatingLastMonth = topRatingLastMonthList,
                Stats = statsList,
                StatsPaging = new()
                {
					Limit = limit,
					Offset = offset,
					HasMore = hasMore,
				},
            });
        }

		// TODO: Add 1v1
		//[Route("1v1")]
		//public async Task<IActionResult> Solo1v1()
		//{
		//    return View("SoloLeaderboard", new SoloViewModel());
		//}

		//public async Task<IActionResult> Index(GameType gameType, CancellationToken cancellationToken, int limit = 100, int offset = 0)
		//{
		//    return await GetTeamVersusLeaderboard(gameType, limit, offset, cancellationToken);
		//}

		//[ActionName("2v2")]
		//public async Task<IActionResult> TeamVersus2v2(
		//    CancellationToken cancellationToken,
		//    long? period = null,
		//    TeamVersusLeaderboardSort sort = TeamVersusLeaderboardSort.Rating,
		//    int limit = 100,
		//    int offset = 0)
		//{
		//    return await Index(GameType.SVS_2v2, cancellationToken, period, sort, limit, offset);
		//}

		//[ActionName("3v3")]
		//public async Task<IActionResult> TeamVersus3v3(
		//    CancellationToken cancellationToken,
		//    long? period = null,
		//    TeamVersusLeaderboardSort sort = TeamVersusLeaderboardSort.Rating,
		//    int limit = 100,
		//    int offset = 0)
		//{
		//    return await Index(GameType.SVS_3v3, cancellationToken, period, sort, limit, offset);
		//}

		//[ActionName("4v4")]
		//public async Task<IActionResult> TeamVersus4v4(
		//    CancellationToken cancellationToken,
		//    long? period = null,
		//    TeamVersusLeaderboardSort sort = TeamVersusLeaderboardSort.Rating,
		//    int limit = 100,
		//    int offset = 0)
		//{
		//    return await Index(GameType.SVS_4v4, cancellationToken, period, sort, limit, offset);
		//}

		// TODO: Add pb
		//[Route("pb")]
		//public async Task<IActionResult> Powerball()
		//{
		//    return View("PowerballLeaderboard", new PowerballViewModel());
		//}
	}
}
