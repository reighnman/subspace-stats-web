using SubspaceStats.Areas.League.Authorization;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.League.Roles
{
    public class AddUserRoleViewModel
    {
        [Display(Name = "User Name")]
        [Required]
        public string? UserName { get; set; }

        [Display(Name = "Role")]
        [Required]
        public LeagueRole? Role { get; set; }
    }
}
