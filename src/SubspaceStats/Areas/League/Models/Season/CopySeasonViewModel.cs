using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SubspaceStats.Areas.League.Models.SeasonGame;
using SubspaceStats.Areas.League.Models.Team;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class CopySeasonViewModel : IValidatableObject
    {
        [ValidateNever]
        public required SeasonDetails SourceSeason { get; set; }

        [ValidateNever]
        public required List<PlayerListItem> SourcePlayers { get; set; }

        [ValidateNever]
        public required OrderedDictionary<long, TeamModel> SourceTeams { get; set; }

        [ValidateNever]
        public required List<GameModel> SourceGames { get; set; }

        [ValidateNever]
        public required OrderedDictionary<int, SeasonRound> SourceRounds { get; set; }

        [Display(Name = "Season Name")]
        [StringLength(128, MinimumLength = 1)]
        [Required]
        public required string SeasonName { get; set; }

        [Display(Name = "Include Players")]
        public bool IncludePlayers { get; set; }

        [Display(Name = "Include Teams")]
        public bool IncludeTeams { get; set; }

        [Display(Name = "Include Games")]
        public bool IncludeGames { get; set; }

        [Display(Name = "Include Rounds")]
        public bool IncludeRounds { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IncludePlayers && !IncludeTeams && !IncludeGames && !IncludeRounds)
            {
                yield return new ValidationResult(
                    "At least one include option is required: Include Players, Include Teams, Include Games, or Include Rounds.");
            }

            if (IncludeTeams && !IncludePlayers)
            {
                yield return new ValidationResult(
                    "Players must be included when including teams.",
                    [nameof(IncludePlayers)]);
            }

            if (IncludeGames && !IncludeTeams)
            {
                yield return new ValidationResult(
                    "Teams must be included when including games.",
                    [nameof(IncludeTeams)]);
            }
        }
    }
}
