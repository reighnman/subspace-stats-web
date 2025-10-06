using SubspaceStats.Areas.League.Models.League;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.Season
{

    public class CreateSeasonModel
    {
        [Display(Name = "Season Name")]
        [StringLength(128, MinimumLength = 1)]
        [Required]
        public required string SeasonName { get; set; }

        [Display(Name = "League ID")]
        [Required]
        public long? LeagueId { get; set; }
    }

    public class CreateSeasonViewModel
    {
        public CreateSeasonModel? Model { get; set; }

        public required List<LeagueModel> Leagues { get; set; }
    }
}
