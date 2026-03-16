using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SubspaceStats.Areas.League.Authorization;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.Season.Roles
{
    public class DeleteUserRoleViewModel : ISeasonViewModel
    {
        [BindNever]
        [ValidateNever]
        public required SeasonDetails SeasonDetails { get; set; }

        public SeasonPage Page => SeasonPage.Roles;

        [BindNever]
        [ValidateNever]
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; set; }

        [Display(Name = "User ID")]
        [Required]
        public string? UserId { get; set; }

        [Display(Name = "User Name")]
        [BindNever]
        [ValidateNever]
        public string? UserName { get; set; }

        [Required]
        [AllowedValues(SeasonRole.Manager)]
        public SeasonRole? Role { get; set; }
    }
}
