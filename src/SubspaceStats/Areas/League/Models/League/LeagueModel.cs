using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.League
{
    public class LeagueModel
    {
        [Display(Name = "League ID")]
        public long Id { get; set; }

        [Display(Name = "League Name")]
        [Required]
        [StringLength(128, MinimumLength = 3)]
        public required string Name { get; set; }

        [Display(Name = "Game Type")]
        [Required]
        public long GameTypeId { get; set; }
    }
}
