using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using SubspaceStats.Areas.Admin.Models.GameTypes;
using SubspaceStats.Areas.League.Authorization;
using SubspaceStats.Models;
using SubspaceStats.Services;

namespace SubspaceStats.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleNames.Administrator)]
    [Area("Admin")]
    public class GameTypesController(
        IStatsRepository statsRepository,
        HybridCache cache) : Controller
    {
        private readonly IStatsRepository _statsRepository = statsRepository;
        private readonly HybridCache _cache = cache;

        // GET Admin/GameTypes
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            return View(
                new GameTypesListViewModel
                {
                    GameTypes = await _statsRepository.GetGameTypesAsync(cancellationToken),
                });
        }

        // GET Admin/GameTypes/Create
        public IActionResult Create()
        {
            return View(
                new GameTypeViewModel
                {
                    GameMode = GameMode.TeamVersus,
                });
        }

        // POST Admin/GameTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameTypeViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _statsRepository.InsertGameTypeAsync(model.GameTypeName!, model.GameMode!.Value, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.GameTypes, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET Admin/GameTypes/Edit/{id}
        public async Task<IActionResult> Edit(long? id, CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            GameType? gameType = await _statsRepository.GetGameTypeAsync(id.Value, cancellationToken);
            if (gameType is null)
            {
                return NotFound();
            }

            return View(
                new GameTypeViewModel
                {
                    GameTypeId = gameType.Id,
                    GameTypeName = gameType.Name,
                    GameMode = gameType.GameMode,
                });
        }

        // POST Admin/GameTypes/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, GameTypeViewModel model, CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            GameType? gameType = await _statsRepository.GetGameTypeAsync(id.Value, cancellationToken);
            if (gameType is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _statsRepository.UpdateGameTypeAsync(id.Value, model.GameTypeName!, model.GameMode!.Value, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.GameTypes, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET Admin/GameTypes/Delete/{id}
        public async Task<IActionResult> Delete(long? id, CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            GameType? gameType = await _statsRepository.GetGameTypeAsync(id.Value, cancellationToken);
            if (gameType is null)
            {
                return NotFound();
            }

            return View(
                new GameTypeViewModel
                {
                    GameTypeId = gameType.Id,
                    GameTypeName = gameType.Name,
                    GameMode = gameType.GameMode,
                });
        }

        // POST Admin/GameTypes/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(long? id, CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            GameType? gameType = await _statsRepository.GetGameTypeAsync(id.Value, cancellationToken);
            if (gameType is null)
            {
                return NotFound();
            }

            await _statsRepository.DeleteGameTypeAsync(id.Value, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.GameTypes, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
