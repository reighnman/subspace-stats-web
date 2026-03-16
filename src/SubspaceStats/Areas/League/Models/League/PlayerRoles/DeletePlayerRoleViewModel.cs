using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SubspaceStats.Areas.League.Authorization;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.League.PlayerRoles
{
    public class DeletePlayerRoleViewModel : ILeagueViewModel
    {
        [Display(Name = "Player Name")]
        [Required]
        public required string PlayerName { get; set; }

        [Required]
        [AllowedValues(LeagueRole.PracticePermit, LeagueRole.PermitManager)]
        public required LeagueRole Role { get; set; }

        public string? Notes { get; set; }

        [BindNever]
        [ValidateNever]
        public required LeagueModel League { get; set; }

        public LeagueSection Section => LeagueSection.Roles;

        [BindNever]
        [ValidateNever]
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
    }
}
