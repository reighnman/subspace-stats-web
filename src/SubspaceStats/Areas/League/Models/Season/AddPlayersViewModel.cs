using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class AddPlayersViewModel
    {
        [Display(Name = "Season ID")]
        public required long SeasonId { get; set; }

        [Display(Name = "Player Names")]
        [DataType(DataType.MultilineText)]
        [Required]
        public required string PlayerNames { get; set; }
    }
}
