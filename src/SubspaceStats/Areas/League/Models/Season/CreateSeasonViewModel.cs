using SubspaceStats.Areas.League.Models.League;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.Season
{

    public class CreateSeasonModel
    {
        [Display(Name = "Season Name")]
        [StringLength(128, MinimumLength = 1)]
        [Required]
        public string? SeasonName { get; set; }
    }

    public class CreateSeasonViewModel
    {
        public required CreateSeasonModel Season { get; set; }

        public required LeagueModel League { get; init; }
    }
}
