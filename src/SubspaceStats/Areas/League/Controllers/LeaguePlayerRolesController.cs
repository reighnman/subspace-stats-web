using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubspaceStats.Areas.League.Authorization;
using SubspaceStats.Areas.League.Models;
using SubspaceStats.Areas.League.Models.League;
using SubspaceStats.Areas.League.Models.League.PlayerRoles;
using SubspaceStats.Models;
using SubspaceStats.Services;
using System.Collections.ObjectModel;
using System.Security.Claims;

namespace SubspaceStats.Areas.League.Controllers
{
    [Area("league")]
    public class LeaguePlayerRolesController(
        IAuthorizationService authorizationService,
        ILeagueRepository leagueRepository) : Controller
    {
        private readonly IAuthorizationService _authorizationService = authorizationService;
        private readonly ILeagueRepository _leagueRepository = leagueRepository;

        private const string AddPlayerRoleMessageKey = "AddPlayerRoleMessage";
        private const string AddPlayerRoleErrorMessageKey = "AddPlayerRoleErrorMessage";

        private static readonly ReadOnlyCollection<LeagueRole> AvailableRolesForManagers;
        private static readonly ReadOnlyCollection<LeagueRole> AvailableRolesForPermitManagers;

        static LeaguePlayerRolesController()
        {
            // Managers (Administrators or League Managers) have full access.
            AvailableRolesForManagers =
                new LeagueRole[] {
                    LeagueRole.PracticePermit,
                    LeagueRole.PermitManager
                }.AsReadOnly();

            // Permit Managers are limited to the 'Practice Permit' role.
            AvailableRolesForPermitManagers =
                new LeagueRole[] {
                    LeagueRole.PracticePermit,
                }.AsReadOnly();
        }

        // GET League/{leagueId}/PlayerRoles
        public async Task<IActionResult> Index(long? leagueId, CancellationToken cancellationToken)
        {
            if (leagueId is null)
            {
                return NotFound();
            }

            LeagueModel? league = await _leagueRepository.GetLeagueAsync(leagueId.Value, cancellationToken); ;
            if (league is null)
            {
                return NotFound();
            }

            IEnumerable<LeagueRole> availableRoles;
            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, league, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    result = await _authorizationService.AuthorizeAsync(User, league, PolicyNames.PermitManager);
                    if (!result.Succeeded)
                    {
                        return Forbid();
                    }

                    availableRoles = AvailableRolesForPermitManagers;
                }
                else
                {
                    return Challenge();
                }
            }
            else
            {
                availableRoles = AvailableRolesForManagers;
            }

            Task<List<LeagueNavItem>> navTask = _leagueRepository.GetLeaguesWithSeasonsAsync(cancellationToken);
            Task<List<LeaguePlayerRoleRequest>> requestsTask = _leagueRepository.GetLeaguePlayerRoleRequestsAsync(
                leagueId.Value,
                availableRoles,
                cancellationToken);
            Task<List<LeaguePlayerRole>> rolesTask = _leagueRepository.GetLeaguePlayerRolesAsync(
                leagueId.Value, 
                availableRoles, 
                cancellationToken);

            await Task.WhenAll(navTask, requestsTask, rolesTask);

            return base.View(
                new LeaguePlayerRolesViewModel
                {
                    League = league,
                    LeagueSeasonChooser = new LeagueSeasonChooserViewModel(navTask.Result, league.Id, null),
                    Requests = requestsTask.Result,
                    Roles = rolesTask.Result,
                    AddPlayerRole = new AddPlayerRoleViewModel()
                    {
                        Role = LeagueRole.PracticePermit,
                        AvailableRoles = availableRoles,
                        Message = TempData[AddPlayerRoleMessageKey] as string,
                        ErrorMessage = TempData[AddPlayerRoleErrorMessageKey] as string,
                    },
                });
        }

        // POST League/{leagueId}/PlayerRoles/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            long? leagueId,
            [Bind(Prefix = "AddPlayerRole")] AddPlayerRoleViewModel model,
            CancellationToken cancellationToken)
        {
            if (leagueId is null)
            {
                return NotFound();
            }

            LeagueModel? league = await _leagueRepository.GetLeagueAsync(leagueId.Value, cancellationToken); ;
            if (league is null)
            {
                return NotFound();
            }

            IEnumerable<LeagueRole> availableRoles;
            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, league, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    result = await _authorizationService.AuthorizeAsync(User, league, PolicyNames.PermitManager);
                    if (!result.Succeeded)
                    {
                        return Forbid();
                    }

                    availableRoles = AvailableRolesForPermitManagers;
                }
                else
                {
                    return Challenge();
                }
            }
            else
            {
                availableRoles = AvailableRolesForManagers;
            }

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.PlayerName) || model.Role is null)
            {
                TempData[AddPlayerRoleErrorMessageKey] = "Invalid input.";
                return RedirectToAction("Index");
            }

            if (!availableRoles.Contains(model.Role.Value))
            {
                return Forbid();
            }

            try
            {
                bool success = await _leagueRepository.InsertLeaguePlayerRoleAsync(
                    leagueId.Value, 
                    model.PlayerName, 
                    model.Role.Value, 
                    User.FindFirstValue(ClaimTypes.NameIdentifier), 
                    null, 
                    cancellationToken);

                if (success)
                {
                    TempData[AddPlayerRoleMessageKey] = $"Successfully assigned '{model.Role.Value.ToDisplayString()}' to '{model.PlayerName}'.";
                }
                else
                {
                    TempData[AddPlayerRoleMessageKey] = $"Player '{model.PlayerName}' already has role '{model.Role.Value.ToDisplayString()}'.";
                }
            }
            catch
            {
                TempData[AddPlayerRoleErrorMessageKey] = $"Error assigning '{model.Role.Value.ToDisplayString()}' to '{model.PlayerName}'.";
            }
            
            return RedirectToAction("Index");
        }

        // GET League/{leagueId}/PlayerRoles/Delete
        public async Task<IActionResult> Delete(
            long? leagueId,
            DeletePlayerRoleViewModel model,
            CancellationToken cancellationToken)
        {
            if (leagueId is null)
            {
                return NotFound();
            }

            LeagueModel? league = await _leagueRepository.GetLeagueAsync(leagueId.Value, cancellationToken);
            if (league is null)
            {
                return NotFound();
            }

            IEnumerable<LeagueRole> availableRoles;
            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, league, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    result = await _authorizationService.AuthorizeAsync(User, league, PolicyNames.PermitManager);
                    if (!result.Succeeded)
                    {
                        return Forbid();
                    }

                    availableRoles = AvailableRolesForPermitManagers;
                }
                else
                {
                    return Challenge();
                }
            }
            else
            {
                availableRoles = AvailableRolesForManagers;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!availableRoles.Contains(model.Role))
            {
                return Forbid();
            }

            model.League = league;
            return View(model);
        }

        // POST League/{leagueId}/PlayerRoles/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(
            long? leagueId,
            DeletePlayerRoleViewModel model,
            CancellationToken cancellationToken)
        {
            if (leagueId is null)
            {
                return NotFound();
            }

            LeagueModel? league = await _leagueRepository.GetLeagueAsync(leagueId.Value, cancellationToken); ;
            if (league is null)
            {
                return NotFound();
            }

            IEnumerable<LeagueRole> availableRoles;
            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, league, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    result = await _authorizationService.AuthorizeAsync(User, league, PolicyNames.PermitManager);
                    if (!result.Succeeded)
                    {
                        return Forbid();
                    }

                    availableRoles = AvailableRolesForPermitManagers;
                }
                else
                {
                    return Challenge();
                }
            }
            else
            {
                availableRoles = AvailableRolesForManagers;
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            if (!availableRoles.Contains(model.Role))
            {
                return Forbid();
            }

            await _leagueRepository.DeleteLeaguePlayerRoleAsync(
                leagueId.Value, 
                model.PlayerName, 
                model.Role, 
                User.FindFirstValue(ClaimTypes.NameIdentifier), 
                model.Notes, 
                cancellationToken);
            
            return RedirectToAction("Index");
        }
    }
}
