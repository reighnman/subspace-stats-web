using Microsoft.AspNetCore.Mvc;
using SubspaceStats.Areas.League.Models.Season;
using SubspaceStats.Areas.League.Models.SeasonPlayer;
using SubspaceStats.Areas.League.Models.Team;
using SubspaceStats.Services;

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
        /// Only allow printable ASCII characters, except colon (colon is used to delimit the target in private messages, and also used to delimit in the chat protocol)
        /// The first character must be a letter or digit.
        /// No consecutive spaces. 
        /// <para>
        /// No leading or trailing spaces. (This is actually handled before checking the regex)
        /// </para>
        /// </remarks>
        /// <returns></returns>
        //[GeneratedRegex("^(?!.* )[A-Za-z0-9][ -~]{0,19}$", RegexOptions.Singleline)]
        //private static partial Regex ValidPlayerNameRegex();

        // GET League/Season/{seasonId}/Players/Add
        public async Task<IActionResult> Add(long seasonId, CancellationToken cancellationToken)
        {
            var seasonTask = _leagueRepository.GetSeasonDetailsAsync(seasonId, cancellationToken);
            var teamsTask = _leagueRepository.GetSeasonTeamsAsync(seasonId, cancellationToken);

            await Task.WhenAll(seasonTask, teamsTask);

            SeasonDetails? seasonDetails = seasonTask.Result;
            if (seasonDetails is null)
            {
                return NotFound();
            }

            return View(
                new AddPlayersViewModel
                {
                    Season = seasonDetails,
                    PlayerNames = "",
                    Teams = teamsTask.Result,
                });
        }

        // POST League/Season/{seasonId}/Players/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(long seasonId, AddPlayersViewModel model, CancellationToken cancellationToken)
        {
            int nameCount = MemoryExtensions.Count(model.PlayerNames, '\n');
            List<string> nameList = new(nameCount);
            int lineNumber = 0;

            foreach (Range range in MemoryExtensions.Split(model.PlayerNames, '\n'))
            {
                lineNumber++;

                // Trim leading or trailing whitespace
                // Trim any Carriage Return that might be remaining at the end (if it was \r\n).
                ReadOnlySpan<char> name = model.PlayerNames[range].Trim().TrimEnd('\r');
                if (name.IsEmpty)
                {
                    // Ignore empty lines.
                    continue;
                }

                if (name.Length > 20)
                {
                    ModelState.AddModelError("", $"Line {lineNumber}: Is longer than 20 characters.");
                    continue;
                }

                int index = name.IndexOfAnyExceptInRange(' ', '~');
                if (index != -1)
                {
                    ModelState.AddModelError("", $"Line {lineNumber}: Contains an invalid character, '{name[index]}'.");
                    continue;
                }

                if (name.Contains(':'))
                {
                    ModelState.AddModelError("", $"Line {lineNumber}: Contains an invalid character, ':'.");
                    continue;
                }

                if (!char.IsAsciiLetterOrDigit(name[0]))
                {
                    ModelState.AddModelError("", $"Line {lineNumber}: The first character must be a letter or digit.");
                    continue;
                }
                
                if (name.Contains("  ", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("", $"Line {lineNumber}: Contains consecutive spaces.");
                    continue;
                }

                //if (!ValidPlayerNameRegex().IsMatch(name))
                //{
                //    ModelState.AddModelError("", $"Line {lineNumber}: Invalid player name.");
                //    continue;
                //}

                nameList.Add(name.ToString());
            }

            if (model.TeamId is not null)
            {
                TeamModel? team = await _leagueRepository.GetTeamAsync(model.TeamId.Value, cancellationToken);
                if (team is null)
                {
                    ModelState.AddModelError(nameof(model.TeamId), "Invalid team.");
                }
            }

            if (!ModelState.IsValid)
            {
                model.Teams = await _leagueRepository.GetSeasonTeamsAsync(seasonId, cancellationToken);
                return View(model);
            }

            await _leagueRepository.InsertSeasonPlayersAsync(seasonId, nameList, model.TeamId, cancellationToken);

            return RedirectToAction("Players", "Season", new { seasonId });
        }


        // GET League/Season/{seasonId}/Players/Edit?playerName={playerName}
        public async Task<IActionResult> Edit(long seasonId, string playerName, CancellationToken cancellationToken)
        {
            // Set is_captain
            // Set is_suspended
            // maybe set team too? or from the team page? might be an easy way to perform a trade

            SeasonPlayer? player = await _leagueRepository.GetSeasonPlayerAsync(seasonId, playerName, cancellationToken);
            if (player is null)
            {
                return NotFound();
            }

            return View(
                new SeasonPlayerViewModel
                {
                    SeasonId = seasonId,
                    Model = player,
                    Teams = await _leagueRepository.GetSeasonTeamsAsync(seasonId, cancellationToken),
                });
        }

        // POST League/Season/{seasonId}/Players/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long seasonId, SeasonPlayer model, CancellationToken cancellationToken)
        {
            SeasonPlayer? player;

            if (!ModelState.IsValid)
            {
                player = await _leagueRepository.GetSeasonPlayerAsync(seasonId, model.PlayerName, cancellationToken);
                if (player is null)
                {
                    return NotFound();
                }

                return View(
                    new SeasonPlayerViewModel
                    {
                        SeasonId = seasonId,
                        Model = model,
                        Teams = await _leagueRepository.GetSeasonTeamsAsync(seasonId, cancellationToken),
                    });
            }

            // The controller actions take player name (hiding PlayerId from the user).
            player = await _leagueRepository.GetSeasonPlayerAsync(seasonId, model.PlayerName, cancellationToken);
            if (player is null)
            {
                return NotFound();
            }

            // but the database functions are based on PlayerId
            model.PlayerId = player.PlayerId;

            await _leagueRepository.UpdateSeasonPlayerAsync(seasonId, model, cancellationToken);
            return RedirectToAction("Players", "Season", new { seasonId });
        }

        // GET League/Season/{seasonId}/Players/Delete?playerName={playerName}
        public async Task<IActionResult> Delete(long seasonId, string playerName, CancellationToken cancellationToken)
        {
            SeasonPlayer? player = await _leagueRepository.GetSeasonPlayerAsync(seasonId, playerName, cancellationToken);
            if (player is null)
            {
                return NotFound();
            }

            return View(
                new SeasonPlayerViewModel
                {
                    SeasonId = seasonId,
                    Model = player,
                    Teams = await _leagueRepository.GetSeasonTeamsAsync(seasonId, cancellationToken),
                });
        }

        // POST League/Season/{seasonId}/Players/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long seasonId, long playerId, CancellationToken cancellationToken)
        {
            await _leagueRepository.DeleteSeasonPlayerAsync(seasonId, playerId, cancellationToken);

            return RedirectToAction("Players", "Season", new { seasonId });
        }
    }
}
