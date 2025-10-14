using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubspaceStats.Areas.League.Authorization;

namespace SubspaceStats.Areas.Admin.Controllers
{
    // TODO: Provide a way to manage the ss.game_type table. Of course, they'll need to configure the game server modules too, but at least no manual database entry.
    [Authorize(Roles = RoleNames.Administrator)]
    [Area("Admin")]
    public class GameTypeController : Controller
    {
        // GET: GameType
        public ActionResult Index()
        {
            return View();
        }

        // GET: GameType/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GameType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GameType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GameType/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GameType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GameType/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GameType/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
