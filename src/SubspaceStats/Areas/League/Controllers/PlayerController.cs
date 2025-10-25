using Microsoft.AspNetCore.Mvc;

namespace SubspaceStats.Areas.League.Controllers
{
    [Area("League")]
    public class PlayerController : Controller
    {
        public IActionResult Index(string playerName, long? seasonId)
        {
            // TODO: should this be a separate page from the non-league area one, /Player?
            // Current Team
            // Previous Teams (if traded)
            // Season Stats
            // Career Stats (other seasons)
            return View();
        }
    }
}
