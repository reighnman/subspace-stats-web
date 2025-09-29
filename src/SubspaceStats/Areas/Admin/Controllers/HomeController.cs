using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;

namespace SubspaceStats.Areas.Admin.Controllers
{
    // TODO: [Authorize("AdminPolicy")]
    [Area("Admin")]
    public class HomeController(
        HybridCache cache) : Controller
    {
        private readonly HybridCache _cache = cache;

        public IActionResult Index()
        {
            return View(TempData["Message"]);
        }

        [HttpPost]
        public async Task<ActionResult> ClearGameTypeCache(CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync(CacheKeys.GameTypes, cancellationToken);

            TempData["Message"] = "Cached Game Type data has been cleared.";

            return RedirectToAction(nameof(Index));
        }
    }
}
