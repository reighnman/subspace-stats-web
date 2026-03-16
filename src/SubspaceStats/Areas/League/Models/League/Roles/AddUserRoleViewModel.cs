using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SubspaceStats.Areas.League.Authorization;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.League.Roles
{
    public class AddUserRoleViewModel
    {
        [Display(Name = "User Name")]
        [Required]
        public string? UserName { get; set; }

        [Required]
        [AllowedValues(LeagueRole.Manager, LeagueRole.PermitManager)]
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
