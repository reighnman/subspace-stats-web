using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SubspaceStats.Areas.League.Models.Season;
using SubspaceStats.Areas.League.Models.Season.Team;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.Season.Player
{
    public class AddPlayersViewModel
    {
        [Display(Name = "Player Names")]
        [DataType(DataType.MultilineText)]
        [Required]
        public required string PlayerNames { get; set; }

        [Display(Name = "Team (optional)")]
        public long? TeamId { get; set; }

        [BindNever]
        [ValidateNever]
        public required SeasonDetails Season { get; set; }

        [BindNever]
        [ValidateNever]
        public required OrderedDictionary<long, TeamModel> Teams { get; set; }
    }
}
