using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SubspaceStats.Areas.League.Models.Team;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.SeasonPlayer
{
    public class AddPlayersViewModel
    {
        [Display(Name = "Season ID")]
        public required long SeasonId { get; set; }

        [Display(Name = "Player Names")]
        [DataType(DataType.MultilineText)]
        [Required]
        public required string PlayerNames { get; set; }

        [Display(Name = "Team (optional)")]
        public long? TeamId { get; set; }

        [ValidateNever]
        public required List<TeamModel> Teams { get; set; }
    }
}
