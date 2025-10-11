using Microsoft.AspNetCore.Mvc;
using SubspaceStats.Areas.League.Models.Season;
using SubspaceStats.Services;

namespace SubspaceStats.Areas.League.Controllers
{
    [Area("League")]
    public class SeasonRoundController(ILeagueRepository leagueRepository) : Controller
    {
        private readonly ILeagueRepository _leagueRepository = leagueRepository;

        // GET League/Season/{seasonId}/Round/Create
        public async Task<IActionResult> Create(long seasonId, CancellationToken cancellationToken)
        {
            var rounds = await _leagueRepository.GetSeasonRoundsAsync(seasonId, cancellationToken);
            int roundNumber = 1;
            foreach(int round in rounds.Keys)
            {
                if (round >= roundNumber)
                    roundNumber = round + 1;
            }

            return View(
                new SeasonRound
                {
                    SeasonId = seasonId,
                    RoundNumber = roundNumber,
                    RoundName = "", // dummy
                });
        }

        // POST League/Season/{seasonId}/Round/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(long seasonId, [Bind("SeasonId", "RoundNumber", "RoundName", "RoundDescription")]SeasonRound seasonRound, CancellationToken cancellationToken)
        {
            if (seasonId != seasonRound.SeasonId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(seasonRound);
            }

            await _leagueRepository.InsertSeasonRoundAsync(seasonRound, cancellationToken);
            return RedirectToAction("Rounds", "Season", new { seasonId });
        }

        // This is called by the validation logic that the [Remote] attribute on SeasonRound.RoundNumber adds.
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> ValidateSeasonRound(long seasonId, int roundNumber, CancellationToken cancellationToken)
        {
            if (await _leagueRepository.GetSeasonRoundAsync(seasonId, roundNumber, cancellationToken) is not null)
            {
                return Json($"Round {roundNumber} already exists.");
            }

            return Json(true);
        }

        // GET League/Season/{seasonId}/Round/{roundNumber}/Edit
        public async Task<ActionResult> Edit(long seasonId, int roundNumber, CancellationToken cancellationToken)
        {
            SeasonRound? seasonRound = await _leagueRepository.GetSeasonRoundAsync(seasonId, roundNumber, cancellationToken);
            if (seasonRound is null)
            {
                return NotFound();
            }

            return View(seasonRound);
        }

        // POST League/Season/{seasonId}/Round/{roundNumber}/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            long seasonId,
            int roundNumber,
            [Bind("SeasonId", "RoundNumber", "RoundName", "RoundDescription")] SeasonRound seasonRound,
            CancellationToken cancellationToken)
        {
            if (seasonId != seasonRound.SeasonId || roundNumber != seasonRound.RoundNumber)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(seasonRound);
            }

            await _leagueRepository.UpdateSeasonRoundAsync(seasonRound, cancellationToken);
            return RedirectToAction("Rounds", "Season", new { seasonId });
        }

        // GET League/Season/{seasonId}/Round/{roundNumber}/Delete
        public async Task<ActionResult> Delete(long seasonId, int roundNumber, CancellationToken cancellationToken)
        {
            SeasonRound? seasonRound = await _leagueRepository.GetSeasonRoundAsync(seasonId, roundNumber, cancellationToken);
            if (seasonRound is null)
            {
                return NotFound();
            }

            return View(seasonRound);
        }

        // POST League/Season/{seasonId}/Round/{roundNumber}/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirm(long seasonId, int roundNumber, CancellationToken cancellationToken)
        {
            await _leagueRepository.DeleteSeasonRoundAsync(seasonId, roundNumber, cancellationToken);
            return RedirectToAction("Rounds", "Season", new { seasonId });
        }
    }
}
