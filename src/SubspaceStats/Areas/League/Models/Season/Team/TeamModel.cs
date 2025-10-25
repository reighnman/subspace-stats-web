using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.Season.Team
{
    public class TeamModel
    {
        [Display(Name = "Team ID")]
        public required long TeamId { get; set; }

        [Display(Name = "Team Name")]
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public required string TeamName { get; set; }

        [Display(Name = "Season ID")]
        public required long SeasonId { get; set; }

        [Display(Name = "Banner (small)")]
        [StringLength(255, MinimumLength = 1)]
        public string? BannerSmall { get; set; }

        [Display(Name = "Banner (large)")]
        [StringLength(255, MinimumLength = 1)]
        public string? BannerLarge { get; set; }

        [Display(Name = "Is Enabled")]
        public bool IsEnabled { get; set; }

        [Display(Name = "Franchise")]
        public long? FranchiseId { get; set; }
    }

    public class TeamWithSeasonInfo : TeamModel
    {
        public required string? FranchiseName { get; set; }
        public required long LeagueId { get; set; }
        public required string LeagueName { get; set; }
        //public required long SeasonId { get; set; }
        public required string SeasonName { get; set; }
        public required int? Wins { get; set; }
        public required int? Losses { get; set; }
        public required int? Draws { get; set; }
    }
}
