using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SubspaceStats.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.Admin.Models.GameTypes
{
    public class GameTypeViewModel : IAdminViewModel
    {
        public AdminSection Section => AdminSection.GameTypes;

        [DisplayName("Game Type ID")]
        [BindNever]
        [ValidateNever]
        public long? GameTypeId { get; set; }

        [DisplayName("Game Type Name")]
        [Required]
        [StringLength(128, MinimumLength = 1)]
        public string? GameTypeName { get; set; }

        [DisplayName("Game Mode")]
        [Required]
        public GameMode? GameMode { get; set; }
    }
}
