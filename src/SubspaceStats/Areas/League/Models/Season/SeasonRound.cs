using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class SeasonRound
    {
        [Display(Name = "Season ID")]
        public required long SeasonId { get; set; }

        [Display(Name = "Round Number")]
        
        [Required]
        [Remote("ValidateSeasonRound", "SeasonRound", "League", AdditionalFields = nameof(SeasonId))]
        public int? RoundNumber { get; set; }

        [Display(Name = "Round Name")]
        [Required]
        [StringLength(128, MinimumLength = 1)]
        public string? RoundName { get; set; }

        [Display(Name = "Round Description")]
        [StringLength(8192)]
        public string? RoundDescription { get; set; }
    }
}
