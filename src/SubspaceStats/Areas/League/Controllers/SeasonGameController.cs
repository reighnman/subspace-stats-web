using Microsoft.AspNetCore.Mvc;

namespace SubspaceStats.Areas.League.Controllers
{
    public class SeasonGameController : Controller
    {
        // GET: SeasonGame
        public ActionResult Index()
        {
            return View();
        }

        // GET: SeasonGame/5/Details
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SeasonGame/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SeasonGame/Create
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

        // GET: SeasonGame/5/Edit
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SeasonGame/5/Edit
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

        // GET: SeasonGame/5/Delete
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SeasonGame/5/Delete
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
