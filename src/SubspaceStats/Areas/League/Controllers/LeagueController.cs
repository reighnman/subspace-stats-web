using Microsoft.AspNetCore.Mvc;
using SubspaceStats.Areas.League.Models.League;
using SubspaceStats.Models;
using SubspaceStats.Services;

namespace SubspaceStats.Areas.League.Controllers
{
    [Area("league")]
    public class LeagueController(
        ILeagueRepository leagueRepository,
        IStatsRepository statsRepository) : Controller
    {
        private readonly ILeagueRepository _leagueRepository = leagueRepository;
        private readonly IStatsRepository _statsRepository = statsRepository;

        // GET: League
        public async Task<ActionResult> Index(CancellationToken cancellationToken)
        {
            Task<List<LeagueModel>> leagueListTask = _leagueRepository.GetLeagueListAsync(cancellationToken);
            Task<OrderedDictionary<long, GameType>> gameTypesTask = _statsRepository.GetGameTypesAsync(cancellationToken);

            await Task.WhenAll(leagueListTask, gameTypesTask);

            return View(
                new LeagueListViewModel()
                {
                    Leagues = leagueListTask.Result,
                    GameTypes = gameTypesTask.Result,
                });
        }

        // GET: League/5/Details
        public async Task<ActionResult> Details(long? id, CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            Task<LeagueModel?> leagueTask = _leagueRepository.GetLeagueAsync(id.Value, cancellationToken);
            Task<List<SeasonListItem>> seasonsTask = _leagueRepository.GetSeasonsAsync(id.Value, cancellationToken);

            LeagueModel? league = await leagueTask;
            if (league is null)
            {
                return NotFound();
            }

            return View(
                new LeagueDetailsViewModel
                {
                    League = league,
                    GameTypes = await _statsRepository.GetGameTypesAsync(cancellationToken),
                    Seasons = await seasonsTask,
                });
        }

        // GET: League/Create
        public async Task<ActionResult> Create(CancellationToken cancellationToken)
        {
            return View(
                new LeagueViewModel()
                {
                    GameTypes = await _statsRepository.GetGameTypesAsync(cancellationToken),
                });
        }

        // POST: League/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Name", "GameTypeId", Prefix ="League")]LeagueModel league, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            long leagueId = await _leagueRepository.InsertLeagueAsync(
                    league.Name!,
                    league.GameTypeId,
                    cancellationToken);

            return RedirectToAction(nameof(Details), "League", new { id = leagueId });
        }

        // GET: League/5/Edit
        public async Task<ActionResult> Edit(long? id, CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }
            
            var league = await _leagueRepository.GetLeagueAsync(id.Value, cancellationToken);
            if (league is null)
            {
                return NotFound();
            }

            return View(
                new LeagueViewModel
                {
                    League = league,
                    GameTypes = await _statsRepository.GetGameTypesAsync(cancellationToken),
                });
        }

        // POST: League/5/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long? id, [Bind("Id", "Name", "GameTypeId", Prefix = "League")] LeagueModel league, CancellationToken cancellationToken)
        {
            if (id is null || id != league.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _leagueRepository.UpdateLeagueAsync(league, cancellationToken);
            return RedirectToAction(nameof(Details));
        }

        // GET: League/5/Delete
        public async Task<ActionResult> Delete(long? id, CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            var league = await _leagueRepository.GetLeagueAsync(id.Value, cancellationToken);
            if (league is null)
            {
                return NotFound();
            }

            return View(
                new LeagueViewModel
                {
                    League = league,
                    GameTypes = await _statsRepository.GetGameTypesAsync(cancellationToken),
                });
        }

        // POST: League/5/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id, CancellationToken cancellationToken)
        {
            await _leagueRepository.DeleteLeagueAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
