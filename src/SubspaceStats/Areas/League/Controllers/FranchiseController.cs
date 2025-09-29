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

        public async Task<ActionResult> Details(long? id, CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            Franchise? franchise = await _leagueRepository.GetFranchiseAsync(id.Value, cancellationToken);
            if (franchise is null)
            {
                return NotFound();
            }

            return View(
                new FranchiseDetails
                {
                    Franchise = franchise,
                    TeamsAndSeasons = await _leagueRepository.GetFranchiseTeamsAsync(id.Value, cancellationToken),
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
            if (ModelState.IsValid)
            {
                long franchiseId = await _leagueRepository.InsertFranchiseAsync(franchise.Name, cancellationToken);
                return RedirectToAction(nameof(Details), new { id = franchiseId });
            }

            return View(franchise);
        }

        public async Task<ActionResult> Edit(long? id, CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            Franchise? franchise = await _leagueRepository.GetFranchiseAsync(id.Value, cancellationToken);
            if(franchise is null)
            {
                return NotFound();
            }

            return View(franchise);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long? id, [Bind("Id", "Name")] Franchise franchise, CancellationToken cancellationToken)
        {
            if (id is null || id != franchise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _leagueRepository.UpdateFranchiseAsync(franchise, cancellationToken);
                return RedirectToAction(nameof(Details), new { id = franchise.Id });
            }

            return View(franchise);
        }

        public async Task<ActionResult> Delete(long? id, CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            Franchise? franchise = await _leagueRepository.GetFranchiseAsync(id.Value, cancellationToken);
            if (franchise is null)
            {
                return NotFound();
            }

            return View(franchise);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id, CancellationToken cancellationToken)
        {
            await _leagueRepository.DeleteFranchiseAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
