using Microsoft.AspNetCore.Mvc;
using SubspaceStats.Areas.League.Models.Franchise;
using SubspaceStats.Services;

namespace SubspaceStats.Areas.League.Controllers
{
    [Area("League")]
    public class FranchiseController(
        ILeagueRepository leagueRepository) : Controller
    {
        private readonly ILeagueRepository _leagueRepository = leagueRepository;

        public async Task<ActionResult> Index(CancellationToken cancellationToken)
        {
            return View(await _leagueRepository.GetFranchiseListAsync(cancellationToken));
        }

        public async Task<ActionResult> Details(long? franchiseId, CancellationToken cancellationToken)
        {
            if (franchiseId is null)
            {
                return NotFound();
            }

            Franchise? franchise = await _leagueRepository.GetFranchiseAsync(franchiseId.Value, cancellationToken);
            if (franchise is null)
            {
                return NotFound();
            }

            return View(
                new FranchiseDetails
                {
                    Franchise = franchise,
                    TeamsAndSeasons = await _leagueRepository.GetFranchiseTeamsAsync(franchiseId.Value, cancellationToken),
                });
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Name")] Franchise franchise, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(franchise);
            }

            long franchiseId = await _leagueRepository.InsertFranchiseAsync(franchise.Name, cancellationToken);
            return RedirectToAction(nameof(Details), new { franchiseId });
        }

        public async Task<ActionResult> Edit(long? franchiseId, CancellationToken cancellationToken)
        {
            if (franchiseId is null)
            {
                return NotFound();
            }

            Franchise? franchise = await _leagueRepository.GetFranchiseAsync(franchiseId.Value, cancellationToken);
            if(franchise is null)
            {
                return NotFound();
            }

            return View(franchise);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long? franchiseId, [Bind("Id", "Name")] Franchise franchise, CancellationToken cancellationToken)
        {
            if (franchiseId is null || franchiseId != franchise.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(franchise);
            }

            await _leagueRepository.UpdateFranchiseAsync(franchise, cancellationToken);
            return RedirectToAction(nameof(Details), new { id = franchise.Id });
        }

        public async Task<ActionResult> Delete(long? franchiseId, CancellationToken cancellationToken)
        {
            if (franchiseId is null)
            {
                return NotFound();
            }

            Franchise? franchise = await _leagueRepository.GetFranchiseAsync(franchiseId.Value, cancellationToken);
            if (franchise is null)
            {
                return NotFound();
            }

            return View(franchise);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long? franchiseId, CancellationToken cancellationToken)
        {
            if (franchiseId is null)
            {
                return NotFound();
            }

            await _leagueRepository.DeleteFranchiseAsync(franchiseId.Value, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
