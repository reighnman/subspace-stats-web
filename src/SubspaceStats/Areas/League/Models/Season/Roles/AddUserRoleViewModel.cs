using SubspaceStats.Areas.League.Authorization;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.Season.Roles
{
    public class AddUserRoleViewModel
    {
        [Display(Name = "User Name")]
        [Required]
        public string? UserName { get; set; }

        [Display(Name = "Role")]
        [Required]
        public SeasonRole? Role { get; set; }
    }
}
