using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SkiaSharp;
using SubspaceStats.Areas.League.Authorization;
using SubspaceStats.Areas.League.Models.Franchise;
using SubspaceStats.Areas.League.Models.Season;
using SubspaceStats.Areas.League.Models.Season.Team;
using SubspaceStats.Options;
using SubspaceStats.Services;

namespace SubspaceStats.Areas.League.Controllers
{
    [Area("League")]
    public class TeamController(
        ILogger<TeamController> logger,
        IAuthorizationService authorizationService,
        IOptions<LeagueOptions> options,
        ILeagueRepository leagueRepository) : Controller
    {
        private readonly ILogger<TeamController> _logger = logger;
        private readonly IAuthorizationService _authorizationService = authorizationService;
        private readonly IOptions<LeagueOptions> _options = options;
        private readonly ILeagueRepository _leagueRepository = leagueRepository;

        // GET League/Team/{teamId}
        public async Task<IActionResult> Index(long teamId, CancellationToken cancellationToken)
        {
            TeamWithSeasonInfo? teamInfo = await _leagueRepository.GetTeamsWithSeasonInfoAsync(teamId, cancellationToken);
            if (teamInfo is null)
            {
                return NotFound();
            }

            var gamesTask = _leagueRepository.GetTeamGames(teamId, cancellationToken);
            var rosterTask = _leagueRepository.GetTeamRoster(teamId, cancellationToken);
            // TODO: Stats of each player

            await Task.WhenAll(gamesTask, rosterTask);

            return View(
                new TeamDetailsViewModel()
                {
                    TeamInfo = teamInfo,
                    GameRecords = gamesTask.Result,
                    Roster = rosterTask.Result,
                });
        }

        // GET League/Season/{seasonId}/Teams/Create
        public async Task<IActionResult> Create(long seasonId, CancellationToken cancellationToken)
        {
            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(seasonId, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, seasonDetails, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return Forbid();
                }
                else
                {
                    return Challenge();
                }
            }

            return View(
                new CreateTeamViewModel
                {
                    Season = seasonDetails,
                    ImageUploadsEnabled = !string.IsNullOrWhiteSpace(_options.Value.ImagePhysicalPath),
                    Franchises = await _leagueRepository.GetFranchisesAsync(cancellationToken)
                });
        }

        // POST League/Season/{seasonId}/Teams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(long seasonId, CreateTeamModel model, CancellationToken cancellationToken)
        {
            //
            // Validate
            //

            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(seasonId, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, seasonDetails, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return Forbid();
                }
                else
                {
                    return Challenge();
                }
            }

            bool imageUploadEnabled = !string.IsNullOrWhiteSpace(_options.Value.ImagePhysicalPath);
            if (!imageUploadEnabled)
            {
                if (model.BannerSmall is not null)
                    ModelState.AddModelError($"Model.{nameof(model.BannerSmall)}", "Image uploads are disabled.");

                if (model.BannerLarge is not null)
                    ModelState.AddModelError($"Model.{nameof(model.BannerLarge)}", "Image uploads are disabled.");
            }

            if (!ModelState.IsValid)
            {
                return View(
                    new CreateTeamViewModel
                    {
                        Model = model,
                        Season = seasonDetails,
                        ImageUploadsEnabled = !string.IsNullOrWhiteSpace(_options.Value.ImagePhysicalPath),
                        Franchises = await _leagueRepository.GetFranchisesAsync(cancellationToken),
                    });
            }

            //
            // Save image files (if any)
            //

            string? bannerSmallPath = null;
            string? bannerLargePath = null;

            if (imageUploadEnabled)
            {
                bool stop = false;
                if (model.BannerSmall is not null)
                {
                    await using Stream inputStream = model.BannerSmall.OpenReadStream();
                    bannerSmallPath = await SaveImageAsync(inputStream, $"Model.{nameof(model.BannerSmall)}");

                    if (bannerSmallPath is null)
                        stop = true; // already failed at saving an image, don't try any more
                }

                if (!stop && model.BannerLarge is not null)
                {
                    await using Stream inputStream = model.BannerLarge.OpenReadStream();
                    bannerLargePath = await SaveImageAsync(inputStream, $"Model.{nameof(model.BannerSmall)}");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(
                    new CreateTeamViewModel
                    {
                        Model = model,
                        Season = seasonDetails,
                        ImageUploadsEnabled = !string.IsNullOrWhiteSpace(_options.Value.ImagePhysicalPath),
                        Franchises = await _leagueRepository.GetFranchisesAsync(cancellationToken),
                    });
            }

            //
            // Insert the database record.
            //

            long teamId = await _leagueRepository.InsertTeamAsync(
                seasonId,
                model.TeamName,
                bannerSmallPath,
                bannerLargePath,
                model.FranchiseId,
                cancellationToken);

            return RedirectToAction("Index", new { teamId });
        }

        // GET League/Teams/{teamId}/Edit
        public async Task<IActionResult> Edit(long teamId, CancellationToken cancellationToken)
        {
            TeamModel? team = await _leagueRepository.GetTeamAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(team.SeasonId, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, seasonDetails, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return Forbid();
                }
                else
                {
                    return Challenge();
                }
            }

            return View(
                new EditTeamViewModel
                {
                    Model = new EditTeamModel {
                        TeamName = team.TeamName,
                        BannerSmallPath = team.BannerSmall,
                        BannerLargePath = team.BannerLarge,
                        FranchiseId = team.FranchiseId,
                    },
                    Season = seasonDetails,
                    ImageUploadsEnabled = !string.IsNullOrWhiteSpace(_options.Value.ImagePhysicalPath),
                    Franchises = await _leagueRepository.GetFranchisesAsync(cancellationToken),
                });
        }

        // POST League/Teams/{teamId}/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long teamId, EditTeamModel model, CancellationToken cancellationToken)
        {
            //
            // Validate
            //

            TeamModel? team = await _leagueRepository.GetTeamAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(team.SeasonId, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, seasonDetails, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return Forbid();
                }
                else
                {
                    return Challenge();
                }
            }

            if (model.FranchiseId is not null
                && await _leagueRepository.GetFranchiseAsync(model.FranchiseId.Value, cancellationToken) is null)
            {
                ModelState.AddModelError($"Model.{nameof(model.FranchiseId)}", "Franchise not found.");
            }

            bool imageUploadEnabled = !string.IsNullOrWhiteSpace(_options.Value.ImagePhysicalPath);
            if (!imageUploadEnabled)
            {
                if (model.BannerSmall is not null)
                    ModelState.AddModelError($"Model.{nameof(model.BannerSmall)}", "Image uploads are disabled.");

                if (model.BannerLarge is not null)
                    ModelState.AddModelError($"Model.{nameof(model.BannerLarge)}", "Image uploads are disabled.");
            }

            if (!ModelState.IsValid)
            {
                return View(
                    new EditTeamViewModel
                    {
                        Model = model,
                        Season = seasonDetails,
                        ImageUploadsEnabled = !string.IsNullOrWhiteSpace(_options.Value.ImagePhysicalPath),
                        Franchises = await _leagueRepository.GetFranchisesAsync(cancellationToken),
                    });
            }

            //
            // Save image files (if any)
            //

            string? bannerSmallPath = null;
            string? bannerLargePath = null;

            if (imageUploadEnabled)
            {
                bool stop = false;
                if (model.BannerSmall is not null)
                {
                    await using Stream inputStream = model.BannerSmall.OpenReadStream();
                    bannerSmallPath = await SaveImageAsync(inputStream, $"Model.{nameof(model.BannerSmall)}");

                    if (bannerSmallPath is null)
                        stop = true; // already failed at saving an image, don't try any more
                }

                if (!stop && model.BannerLarge is not null)
                {
                    await using Stream inputStream = model.BannerLarge.OpenReadStream();
                    bannerLargePath = await SaveImageAsync(inputStream, $"Model.{nameof(model.BannerSmall)}");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(
                    new EditTeamViewModel
                    {
                        Model = model,
                        Season = seasonDetails,
                        ImageUploadsEnabled = !string.IsNullOrWhiteSpace(_options.Value.ImagePhysicalPath),
                        Franchises = await _leagueRepository.GetFranchisesAsync(cancellationToken),
                    });
            }

            await _leagueRepository.UpdateTeamAsync(teamId, model.TeamName, bannerSmallPath, bannerLargePath, model.FranchiseId, cancellationToken);

            return RedirectToAction("Index");
        }

        // GET League/Teams/{teamId}/Delete
        public async Task<IActionResult> Delete(long teamId, CancellationToken cancellationToken)
        {
            TeamModel? team = await _leagueRepository.GetTeamAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(team.SeasonId, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, seasonDetails, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return Forbid();
                }
                else
                {
                    return Challenge();
                }
            }

            FranchiseModel? franchise = (team.FranchiseId is null) ? null : await _leagueRepository.GetFranchiseAsync(team.FranchiseId.Value, cancellationToken);

            return View(
                new DeleteTeamViewModel
                {
                    Model = team,
                    Season = seasonDetails,
                    Franchise = franchise,
                });
        }

        // POST League/Teams/{teamId}/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(long teamId, CancellationToken cancellationToken)
        {
            TeamModel? team = await _leagueRepository.GetTeamAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            SeasonDetails? seasonDetails = await _leagueRepository.GetSeasonDetailsAsync(team.SeasonId, cancellationToken);
            if (seasonDetails is null)
            {
                return NotFound();
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(User, seasonDetails, PolicyNames.Manager);
            if (!result.Succeeded)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return Forbid();
                }
                else
                {
                    return Challenge();
                }
            }

            await _leagueRepository.DeleteTeamAsync(teamId, cancellationToken);
            return RedirectToAction("Teams", "Season", new { seasonId = team.SeasonId });
        }

        private async Task<string?> SaveImageAsync(Stream inputStream, string modelStateKey)
        {
            if (string.IsNullOrWhiteSpace(_options.Value.ImagePhysicalPath))
                return null;

            string? extension = _options.Value.ImageUploadFormat switch
            {
                SKEncodedImageFormat.Bmp => ".bmp",
                SKEncodedImageFormat.Gif => ".gif",
                SKEncodedImageFormat.Ico => ".ico",
                SKEncodedImageFormat.Jpeg => ".jpg",
                SKEncodedImageFormat.Png => ".png",
                SKEncodedImageFormat.Wbmp => ".wbmp",
                SKEncodedImageFormat.Webp => ".webp",
                SKEncodedImageFormat.Pkm => ".pkm",
                SKEncodedImageFormat.Ktx => ".ktx",
                SKEncodedImageFormat.Astc => ".astc",
                SKEncodedImageFormat.Dng => ".dng",
                SKEncodedImageFormat.Heif => ".heif",
                SKEncodedImageFormat.Avif => ".avif",
                SKEncodedImageFormat.Jpegxl => ".jxl",
                _ => null,
            };

            if (extension is null)
            {
                _logger.LogError("Error saving image. Unsupported file format {ImageUploadFormat}.", _options.Value.ImageUploadFormat);
                return null;
            }

            string fileName = Path.ChangeExtension(Guid.CreateVersion7().ToString("N"), extension);
            string filePath = Path.Join(_options.Value.ImagePhysicalPath, "TeamBanners", fileName);

            try
            {
                string? directoryPath = Path.GetDirectoryName(filePath);
                if (string.IsNullOrWhiteSpace(directoryPath))
                    return null;

                Directory.CreateDirectory(directoryPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving image to {ImageFilePath}. Unable to create directory.", filePath);
                ModelState.AddModelError(modelStateKey, "Error saving file.");
                return null;
            }
            
            try
            {
                await using FileStream outputStream = new(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, true);
                using SKBitmap bitmap = SKBitmap.Decode(inputStream);
                if (!bitmap.Encode(outputStream, _options.Value.ImageUploadFormat, 100))
                {
                    _logger.LogError("Error saving image to {ImageFilePath}. Encode returned false.", filePath);
                    ModelState.AddModelError(modelStateKey, $"Error encoding image to {_options.Value.ImageUploadFormat}.");
                    return null;
                }

                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving image to {ImageFilePath}.", filePath);
                ModelState.AddModelError(modelStateKey, "Error saving file.");
                return null;
            }
        }
    }
}
