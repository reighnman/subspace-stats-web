using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SubspaceStats.Areas.League.Authorization;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.League.PlayerRoles
{
    public class AddPlayerRoleViewModel
    {
        [Display(Name = "Player Name")]
        [Required]
        public string? PlayerName { get; set; }

        [Required]
        [AllowedValues(LeagueRole.PracticePermit, LeagueRole.PermitManager)]
        public LeagueRole? Role { get; set; }

        [BindNever]
        [ValidateNever]
        public IEnumerable<LeagueRole>? AvailableRoles { get; set; }

        [BindNever]
        public string? Message { get; set; }

        [BindNever]
        public string? ErrorMessage { get; set; }
    }
}
