using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class SeasonModel
    {
        [Display(Name = "Season ID")]
        public long SeasonId { get; set; }

        [Display(Name = "Season Name")]
        [StringLength(128, MinimumLength = 1)]
        [Required]
        public required string SeasonName { get; set; }

        [Display(Name = "League ID")]
        [Required]
        public required long LeagueId { get; set; }
    }
}
