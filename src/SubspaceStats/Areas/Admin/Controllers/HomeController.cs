using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using SubspaceStats.Areas.Admin.Models;
using SubspaceStats.Areas.League.Authorization;

namespace SubspaceStats.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleNames.Administrator)]
    [Area("Admin")]
    public class HomeController(
        HybridCache cache) : Controller
    {
        private readonly HybridCache _cache = cache;

        public IActionResult Index()
        {
            return View(
                new HomeViewModel
                {
                    Message = TempData["Message"] as string,
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ClearGameTypeCache(CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync(CacheKeys.GameTypes, cancellationToken);

            TempData["Message"] = "Cached Game Type data has been cleared.";

            return RedirectToAction(nameof(Index));
        }
    }
}
