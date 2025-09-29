using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SubspaceStats.Models;
using SubspaceStats.Models.Home;
using SubspaceStats.Models.Leaderboard;
using SubspaceStats.Options;
using SubspaceStats.Services;
using System.Diagnostics;

namespace SubspaceStats.Controllers;

public class HomeController(
    IOptions<StatOptions> options,
    IStatsRepository statsRepository) : Controller
{
    private readonly StatOptions _options = options.Value;
    private readonly IStatsRepository _statsRepository = statsRepository;

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        List<Task<(StatPeriod Period, List<TopRatingRecord> TopRatingList)?>> topRatingTasks = new(_options.Home.Ratings.Count);
        foreach (var ratingSettings in _options.Home.Ratings)
        {
            topRatingTasks.Add(
                _statsRepository.GetTopPlayersByRating(
                    ratingSettings.GameType,
                    ratingSettings.PeriodType,
                    ratingSettings.Top,
                    cancellationToken));
        }

        await Task.WhenAll(topRatingTasks);

        List<(StatPeriod Period, List<TopRatingRecord> TopRatingList)> topRatings = new(_options.Home.Ratings.Count);
        foreach (var task in topRatingTasks)
        {
            (StatPeriod Period, List<TopRatingRecord> TopRatingList)? topRatingInfo = task.Result;

            if (topRatingInfo is not null)
            {
                topRatings.Add(topRatingInfo.Value);
            }
        }

        return View(
            new HomeViewModel
            {
                TopRatings = topRatings,
                GameTypes = await _statsRepository.GetGameTypesAsync(cancellationToken),
            });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}