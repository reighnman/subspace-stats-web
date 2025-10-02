using Microsoft.AspNetCore.Mvc;
using SubspaceStats.Areas.League.Models.Season;
using SubspaceStats.Services;
using System.Text.RegularExpressions;

namespace SubspaceStats.Areas.League.Controllers
{
    [Area("League")]
    public partial class SeasonPlayerController(ILeagueRepository leagueRepository) : Controller
    {
        private readonly ILeagueRepository _leagueRepository = leagueRepository;

        /// <summary>
        /// Regular expression for validating a player name.
        /// </summary>
        /// <remarks>
        /// 1 - 20 characters in length.
        /// Must start with a letter or number
        /// Only allow printable ASCII characters, except colon (colon is used to delimit the target in private messages, and also used to delimit in the chat protocol)
        /// The first character must be a letter or digit.
        /// No consecutive spaces. 
        /// <para>
        /// No leading or trailing spaces. (This is actually handled before checking the regex)
        /// </para>
        /// </remarks>
        /// <returns></returns>
        [GeneratedRegex("^(?=.{1,20}$)[a-zA-Z0-9](?!.*  )[ -;=?-~]*$")]
        private static partial Regex ValidPlayerNameRegex();

        // GET League/Season/{seasonId}/Player/Add
        public IActionResult Add(long seasonId, CancellationToken cancellationToken)
        {
            return View(
                new AddPlayersViewModel
                {
                    SeasonId = seasonId,
                    PlayerNames = "",
                });
        }

        // POST League/Season/{seasonId}/Player/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(long seasonId, AddPlayersViewModel model, CancellationToken cancellationToken)
        {
            int nameCount = MemoryExtensions.Count(model.PlayerNames, '\n');
            List<string> nameList = new(nameCount);
            int lineNumber = 0;

            foreach (Range range in MemoryExtensions.Split(model.PlayerNames, '\n'))
            {
                lineNumber++;

                ReadOnlySpan<char> name = model.PlayerNames[range].Trim().Trim('\r');
                if (name.IsEmpty)
                {
                    // Ignore empty lines.
                    continue;
                }

                if (name.Length > 20)
                {
                    ModelState.AddModelError("", $"Line {lineNumber}: A player name cannot be longer than 20 characters.");
                    continue;
                }

                
                if (!ValidPlayerNameRegex().IsMatch(name))
                {
                    ModelState.AddModelError("", $"Line {lineNumber}: Invalid player name.");
                    continue;
                }

                nameList.Add(name.ToString());
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //_leagueRepository.AddSeasonPlayersAsync(seasonId, nameList, cancellationToken);

            return RedirectToAction("Players", "Season", new { seasonId });
        }


        // GET League/Season/{seasonId}/Player/Edit?playerName={playerName}
        public IActionResult Edit(long? seasonId, string playerName, CancellationToken cancellationToken)
        {
            // Set is_captain
            // Set is_suspended
            // maybe set team too? or from the team page?

            return View();
        }

        // GET League/Season/{seasonId}/Player/Delete?playerName={playerName}
        public IActionResult Delete(long? seasonId, string playerName, CancellationToken cancellationToken)
        {
            // remove signup
            return View();
        }
    }
}
